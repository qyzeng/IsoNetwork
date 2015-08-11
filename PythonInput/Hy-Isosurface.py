#!/usr/bin/env python
import math
import numpy
import scipy.special
import scipy.misc
import pyqtgraph as pg
import json


x,y,z = numpy.ogrid[-50:50:65j,-50:50:65j,-50:50:65j]
a0=1

r = lambda x,y,z: numpy.sqrt(x**2+y**2+z**2)
theta = lambda x,y,z: numpy.arccos(z/r(x,y,z))
phi = lambda x,y,z: numpy.arctan2(y,x)

R = lambda r,n,l: numpy.sqrt(((2.0/n/a0)**3)*(math.factorial(n-l-1))/(math.factorial(n+l))/2/n)*(2*r/n/a0)**l * numpy.exp(-r/n/a0) * scipy.special.genlaguerre(n-l-1,2*l+1)(2*r/n/a0)
WF = lambda r,theta,phi,n,l,m: R(r,n,l) * scipy.special.sph_harm(m,l,phi,theta)
absWF = lambda r,theta,phi,n,l,m: abs((WF(r,theta,phi,n,l,m))**2)

def CalculatePhase(verts):
    iso_phase0 = [None]*int(verts.size/3)
    for iwphase in range (0, int(verts.size/3)):
        iso_r = numpy.sqrt(verts[iwphase][0]**2+verts[iwphase][1]**2+verts[iwphase][2]**2)
        iso_theta = numpy.arccos(verts[iwphase][2]/iso_r)
        iso_phi = numpy.arctan2(verts[iwphase][1],verts[iwphase][0])
        #iso_R = numpy.sqrt(((2.0/n/a0)**3)*(math.factorial(n-l-1))/(math.factorial(n+l))/2/n)*(2*iso_r/n/a0)**l * numpy.exp(-iso_r/n/a0) * scipy.special.genlaguerre(n-l-1,2*l+1)(2*iso_r/n/a0)
        iso_WF = scipy.special.sph_harm(m,l,iso_phi,iso_theta)
        #iso_WF = iso_R * scipy.special.sph_harm(m,l,iso_phi,iso_theta)
        iso_phase0[iwphase]=numpy.angle(iso_WF)
    iso_phase = numpy.array(iso_phase0)
    return iso_phase

def GetRealWF(n,l,m,isovalue):
    wf = WF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    realwf = numpy.real (wf)*10
    #realwf = numpy.fabs(numpy.real(wf)*10)
    realwf[numpy.isnan(realwf)]=0
    f_isovalue = isovalue*numpy.max(realwf)*0.2
    verts1, faces1 = pg.isosurface(realwf,f_isovalue)
    verts2, faces2 = pg.isosurface(realwf,-f_isovalue)
    verts0 = numpy.vstack ((verts1,verts2))
    faces20 = faces2+int(verts1.size/3)
    faces = numpy.vstack ((faces1,faces20))
    verts = verts0*2*50/(65-1)-(50,50,50)
    phases = CalculatePhase(verts)
    return verts, faces, phases

def GetAbsWF(n,l,m,isovalue):
    abswf = absWF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    abswf[numpy.isnan(abswf)]=0
    f_isovalue = isovalue*numpy.max(abswf)*0.2
    verts0, faces = pg.isosurface(abswf,f_isovalue)
    verts = verts0*2*50/(65-1)-(50,50,50)
    phases = CalculatePhase(verts)
    return verts, faces, phases

def CreatejsonData(verts, faces, phases,isovalue):
    cmd = 'creat_iso'
    num_vertices = int(verts.size/3)
    num_triangles = int(faces.size/3)
    
    list_vertices =''
    list_triangles =''
    list_phases =''
    for i in range(0,num_vertices):
        x = verts[i][0]
        y = verts[i][1]
        z = verts[i][2]
        list_vertices = list_vertices+"%s|%s|%s|" %(x,y,z)

    for i in range(0,num_triangles):
        v1 = faces[i][0]
        v2 = faces[i][1]
        v3 = faces[i][2]
        list_triangles = list_triangles+"%s|%s|%s|" %(v1,v2,v3)

    for i in range(0, num_vertices):
        list_phases = list_phases+"%s|" %(phases[i])
                    
    message = {'cmd':cmd,
    'num_vertices':num_vertices,
    'num_triangles':num_triangles,
    'list_vertices':list_vertices,
    'list_triangles':list_triangles,
    'list_phases':list_phases,
    'isovalue':isovalue}
    return message

def CreateIsoMessage(verts, faces, phases,isovalue):
    num_vertices = int(verts.size/3)
    num_triangles = int(faces.size/3)

    message= 'R|%s|%s' %(num_vertices,num_triangles)
    for i in range(0,num_vertices):
        x = 0 if verts[i][0] is 'nan' else verts[i][0]
        y = 0 if verts[i][1] is 'nan' else verts[i][1]
        z = 0 if verts[i][2] is 'nan' else verts[i][2]
        message = message+"|%s|%s|%s" %(x,y,z)

    for i in range(0,num_triangles):
        v1 = faces[i][0]
        v2 = faces[i][1]
        v3 = faces[i][2]
        message = message+"|%s|%s|%s" %(v1,v2,v3)

    message = message +"|%s"%(isovalue);
    for i in range(0,num_vertices):
        message = message+"|%s" %(phases[i])
    
    return message

n=5
l=4
m=2
isovalue = 0.5
verts,faces,iso_phase = GetRealWF(n,l,m,isovalue)
message = CreateIsoMessage(verts, faces, iso_phase,isovalue)
#jsondata = {'cmd':"R",
#            'message':CreatejsonData(verts,faces,iso_phase,isovalue)}

import socket
import sys
import random
import time
import math
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ('localhost', 52111)

sock.connect(server_address)
sock.sendall(message+"\r\n")
print >>sys.stderr, 'starting up on %s port %s' % server_address
sock.bind(server_address)
# Listen for incoming connections
sock.listen(1)
while True:
    # Wait for a connection
    print >>sys.stderr, 'waiting for a connection'
    connection, client_address = sock.accept()
    try:
        print >>sys.stderr, 'connection from', client_address

        # Receive the data in small chunks and retransmit it
        while True:
            data = connection.recv(16)
            
            print >>sys.stderr, 'received "%s"' % data
            if data:
                print >>sys.stderr, 'sending data back to the client'
            else:
                print >>sys.stderr, 'no more data from', client_address
                break

            connection.sendall(message+"\r\n")
            #message2 = 'c|%s|%s|%s' %(5,2,5)
            #connection.sendall(message2+"\r\n")
            #connection.sendall(json.dumps(jsondata)+"\r\n")    
            
    finally:
        # Clean up the connection
        connection.close()
