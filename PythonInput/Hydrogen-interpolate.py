#!/usr/bin/env python
import math
import numpy
import scipy.special
import scipy.misc
import pyqtgraph as pg

def linterp1d(value1,value2,step):
    return value1+(value2-value1)*step

def linterp2d(value11,value21,value12,value22,step1,step2):
    return linterp1d(linterp1d(value11,value21,step1),linterp1d(value12,value22,step1),step2)

def complexphi (x,y):
    if x>0:
        if y>0:
            return numpy.arctan(y/x)
        else:
            return 2*numpy.pi+numpy.arctan(y/x)
    if x ==0:
        if y>0:
            return .5*numpy.pi
        else:
            return 1.5*numpy.pi
    if x < 0:
            return numpy.pi+numpy.arctan(y/x)

#phase is the base matrix,vertices is the points need to be found
def linterp3d(mphase,mverts):
    Nvertices = int(mverts.size/3)
    cphase = [None]*Nvertices
    for i in range (0,Nvertices):
    
        xsize = int(mphase.size/mphase[0].size)
        ysize = int(mphase[0].size/mphase[0][0].size)
        zsize =  int(mphase[0][0].size)
    
        vertx = int(mverts[i][0])
        verty = int(mverts[i][1])
        vertz = int(mverts[i][2])
    
        stepx = mverts[i][0]-int(mverts[i][0])
        stepy = mverts[i][1]-int(mverts[i][1])
        stepz = mverts[i][2]-int(mverts[i][2])
        if vertx == xsize-1 and verty == xsize-1 and vertz == xsize-1:
            cphase[i]= mphase[vertx][verty][vertz]
            continue
        if vertx == int(xsize-1):
            if verty == int(ysize-1):
                value001 = mphase[vertx][verty][vertz]
                value002 = mphase[vertx][verty][vertz+1]
                interp = linterp1d(value001,value002,stepz)
                cphase[i]=(interp)
                continue
            value011 = mphase[vertx][verty][vertz]
            value021 = mphase[vertx][verty+1][vertz]
            value012 = mphase[vertx][verty][vertz+1]
            value022 = mphase[vertx][verty+1][vertz+1]
            interp = linterp2d(value011,value021,value012,value022,stepy,stepz)
            cphase[i]=(interp)
            continue
        if verty == int(ysize-1):
            if vertz == int(zsize-1):
                value100 = mphase[vertx][verty][vertz]
                value200 = mphase[vertx+1][verty][vertz]
                interp = linterp1d(value100,value200,stepx)
                cphase[i]=(interp)
                continue
            value101 = mphase[vertx][verty][vertz]
            value201 = mphase[vertx+1][verty][vertz]
            value102 = mphase[vertx][verty][vertz+1]
            value202 = mphase[vertx+1][verty][vertz+1]
            interp = linterp2d(value101,value201,value102,value202,stepx,stepz)
            cphase[i]=(interp)
            continue
        if vertz == int(zsize-1):
            value110 = mphase[vertx][verty][vertz]
            value210 = mphase[vertx+1][verty][vertz]
            value120 = mphase[vertx][verty+1][vertz]
            value220 = mphase[vertx+1][verty+1][vertz]
            interp = linterp2d(value110,value210,value120,value220,stepx,stepy)
            cphase[i]=(interp)
            continue
        value111 = mphase[vertx][verty][vertz]
        value211 = mphase[vertx+1][verty][vertz]
        value121 = mphase[vertx][verty+1][vertz]
        value221 = mphase[vertx+1][verty+1][vertz]
        interp1 = linterp2d(value111,value211,value121,value221,stepx,stepy)
        value112 = mphase[vertx][verty][vertz+1]
        value212 = mphase[vertx+1][verty][vertz+1]
        value122 = mphase[vertx][verty+1][vertz+1]
        value222 = mphase[vertx+1][verty+1][vertz+1]
        interp2 = linterp2d(value112,value212,value122,value222,stepx,stepy)
        interp3 = linterp1d(interp1,interp2,stepz)
        cphase[i]=(interp3)
    return numpy.array(cphase)
    

x,y,z = numpy.mgrid[-50:50:51j,-50:50:51j,-50:50:51j]
def GetWaveFunction(n,l,m,x,y,z):
    r = lambda x,y,z: numpy.sqrt(x**2+y**2+z**2)
    theta = lambda x,y,z: 0 if r(x,y,z) is 0 else numpy.arccos(z/r(x,y,z))
    phi = lambda x,y,z: numpy.arctan2(y,x)
    #phi = lambda x,y,z: numpy.arctan(y/x)
    a0 = 1.
    R = lambda r,n,l: numpy.sqrt(((2.0/n/a0)**3)*(math.factorial(n-l-1))/(math.factorial(n+l))/2/n)*(2*r/n/a0)**l * numpy.exp(-r/n/a0) * scipy.special.genlaguerre(n-l-1,2*l+1)(2*r/n/a0)
    #R = lambda r,n,l: (2*r/n/a0)**l * numpy.exp(-r/n/a0) * scipy.special.genlaguerre(n-l-1,2*l+1)(2*r/n/a0)
    WF = lambda r,theta,phi,n,l,m: R(r,n,l) * scipy.special.sph_harm(m,l,phi,theta)
    absWF = lambda r,theta,phi,n,l,m: abs((WF(r,theta,phi,n,l,m))**2)
    wf = WF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    w = absWF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    w[numpy.isnan(w)]=0
    #wf[numpy.isnan(wf)]=0

    phase = numpy.angle(wf)
    #realwf = numpy.imag(wf)*10
    #realwf = numpy.real(wf)*10
    realwf = numpy.fabs(numpy.real(wf)*10)# need to by 10 because of the pg.isosurface() function
    #phase = numpy.array(w)
    #xsize = w.size/w[0].size
    #ysize = w[0].size/w[0][0].size
    #zsize = w[0][0].size
    #for i in range(0,xsize):
    #    for j in range(0,ysize):
    #        for k in range(0,zsize):
    #            #angular = numpy.arctan2(wf[i][j][k].imag,wf[i][j][k].real)
    #            #if angular < 0:
    #            #    phase[i][j][k] = angular+2*numpy.pi
    #            #else:
    #            #    phase[i][j][k] = angular
    #            phase[i][j][k] = numpy.arctan2(wf[i][j][k].imag,wf[i][j][k].real)
    phase[numpy.isnan(phase)]=0
    realwf[numpy.isnan(realwf)]=0
    return w,phase, realwf
absw,wphase,realwf = GetWaveFunction(5,4,4,x,y,z)
isovalue = 0.5
#f_isovalue = #isovalue*0.0000001# need to select a proper value
f_isovalue = isovalue*numpy.max(realwf)*0.2
#verts0, faces = pg.isosurface(absw,f_isovalue)#the verts are depending on ogrid index nor ogrid value
verts0, faces = pg.isosurface(realwf,f_isovalue)
verts1, facesm = pg.isosurface(realwf,-f_isovalue)
#verts0, faces = pg.isosurface(absw,f_isovalue)
#isophase0 = linterp3d(wphase,verts0)

xx=numpy.linspace(0,50,51)
yy=numpy.linspace(0,50,51)
zz=numpy.linspace(0,50,51)
from scipy.interpolate import griddata
from scipy.interpolate import RegularGridInterpolator
from scipy.interpolate import interpn
#inter_function = griddata((xx,yy,zz),wphase)
#isophase = griddata((xx,yy,zz),wphase,verts0,method='linear')
#inter_function = RegularGridInterpolator((xx,yy,zz),wphase)
isophase = interpn((xx,yy,zz),wphase,verts0,method = 'linear')#both RegularGridInterpolator and interpn can do the interpolation
isophasem = interpn((xx,yy,zz),wphase,verts1,method = 'linear')
#isophase = inter_function(verts0)
#isophasem = inter_function(verts1)
#numpy.savetxt('isop',isophase)

verts = verts0*2-(50,50,50)
N_vertices = int(verts.size/3)
N_triangles = int(faces.size/3)
N_isophase = int (isophase.size)

vertsm = verts1*2-(50,50,50)
N_verticesm = int(vertsm.size/3)
N_trianglesm = int(facesm.size/3)
N_isophasem = int (isophasem.size)

import socket
import sys
import random
import time
import math
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ('localhost', 51111)
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
            #connection.sendall("L,simple_coordinates\r\n")
            #data = connection.recv(16)
            #connection.sendall('C,0,0,0,1\r\n')
            #generate a egg carton mesh
            message= 'R|%s|%s' %(N_vertices,N_triangles)
            for i in range(0,N_vertices):
                x = 0 if verts[i][0] is 'nan' else verts[i][0]
                y = 0 if verts[i][1] is 'nan' else verts[i][1]
                z = 0 if verts[i][2] is 'nan' else verts[i][2]
                message = message+"|%s|%s|%s" %(x,y,z)

            for i in range(0,N_triangles):
                v1 = faces[i][0]
                v2 = faces[i][1]
                v3 = faces[i][2]
                message = message+"|%s|%s|%s" %(v1,v2,v3)
            connection.sendall(message+"\r\n")
            mesh_no = connection.recv(16)
            print >>sys.stderr, 'Object Number Mesh: "%s"' % (mesh_no)
            message = 'd|%s|%s|%s'%(int(mesh_no),isovalue,N_isophase)
            for i in range(0, N_isophase):
                message = message+"|%s" %(isophase[i])
            connection.sendall(message+"\r\n")
            #message = 'c|%s|%s|%s|%s' %(int(mesh_no),20,20,10)
            #connection.sendall(message+"\r\n")
            message= 'R|%s|%s' %(N_verticesm,N_trianglesm)
            for i in range(0,N_verticesm):
                x = 0 if vertsm[i][0] is 'nan' else vertsm[i][0]
                y = 0 if vertsm[i][1] is 'nan' else vertsm[i][1]
                z = 0 if vertsm[i][2] is 'nan' else vertsm[i][2]
                message = message+"|%s|%s|%s" %(x,y,z)

            for i in range(0,N_trianglesm):
                v1 = facesm[i][0]
                v2 = facesm[i][1]
                v3 = facesm[i][2]
                message = message+"|%s|%s|%s" %(v1,v2,v3)
            connection.sendall(message+"\r\n")
            mesh_no = connection.recv(16)
            print >>sys.stderr, 'Object Number Mesh: "%s"' % (mesh_no)
            message = 'd|%s|%s|%s'%(int(mesh_no),isovalue,N_isophasem)
            for i in range(0, N_isophasem):
                message = message+"|%s" %(isophasem[i])
            connection.sendall(message+"\r\n")
            message = 'c|%s|%s|%s|%s' %(int(mesh_no),20,20,10)
            connection.sendall(message+"\r\n")
            data = connection.recv(16)
            #animate the mesh data
    finally:
        # Clean up the connection
        connection.close()

