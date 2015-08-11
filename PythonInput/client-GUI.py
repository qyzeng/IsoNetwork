import math
import numpy
import scipy.special
import scipy.misc
import pyqtgraph as pg
import json
import time

def SetGrid(Nx,Dn):
    cx = complex(0,Nx)
    return numpy.ogrid[-Dn:Dn:cx,-Dn:Dn:cx,-Dn:Dn:cx]

#x,y,z = numpy.ogrid[-50:50:51j,-50:50:51j,-50:50:51j]
#x,y,z
a0=1
r = lambda x,y,z: numpy.sqrt(x**2+y**2+z**2)
theta = lambda x,y,z: numpy.arccos(z/r(x,y,z))
phi = lambda x,y,z: numpy.arctan2(y,x)

R = lambda r,n,l: numpy.sqrt(((2.0/n/a0)**3)*(math.factorial(n-l-1))/(math.factorial(n+l))/2/n)*(2*r/n/a0)**l * numpy.exp(-r/n/a0) * scipy.special.genlaguerre(n-l-1,2*l+1)(2*r/n/a0)
WF = lambda r,theta,phi,n,l,m: R(r,n,l) * scipy.special.sph_harm(m,l,phi,theta)
absWF = lambda r,theta,phi,n,l,m: abs((WF(r,theta,phi,n,l,m))**2)

def CalculatePhase(verts,m,l):
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

def GetRealWF(n,l,m,isovalue,Nx,Dn):
    x,y,z = SetGrid(Nx,Dn)
    wf = WF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    realwf = numpy.real (wf)*10
    realwf[numpy.isnan(realwf)]=0
    f_isovalue = isovalue*numpy.max(realwf)*0.2
    verts1, faces1 = pg.isosurface(realwf,f_isovalue)
    verts2, faces2 = pg.isosurface(realwf,-f_isovalue)
    verts0 = numpy.vstack ((verts1,verts2))
    faces20 = faces2+int(verts1.size/3)
    faces = numpy.vstack ((faces1,faces20))
    verts = verts0*2*Dn/(Nx-1)-(Dn,Dn,Dn)
    phases = CalculatePhase(verts,m,l)
    return verts, faces, phases

def GetAbsWF(n,l,m,isovalue,Nx,Dn):
    x,y,z = SetGrid(Nx,Dn)
    abswf = absWF(r(x,y,z),theta(x,y,z),phi(x,y,z),n,l,m)
    abswf[numpy.isnan(abswf)]=0
    f_isovalue = isovalue*numpy.max(abswf)*0.2
    verts0, faces = pg.isosurface(abswf,f_isovalue)
    verts = verts0*2*Dn/(Nx-1)-(Dn,Dn,Dn)
    phases = CalculatePhase(verts,m,l)
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

def CreateIsoMessageWithoutPhase(verts, faces):
    print "a"
    num_vertices = int(verts.size/3)
    num_triangles = int(faces.size/3)

    message= 'W|%s|%s' %(num_vertices,num_triangles)
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
    
    return message

import socket
import sys
from Tkinter import *

def SendMessageBySocket(message):
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_address = ('localhost', 53111)
    indicator = sock.connect_ex(server_address)
    if indicator is 0:
        sock.sendall(message+"\n")
        sock.shutdown (1)
        sock.close ()
    else:
        print "No connection could be made because the target machine actively refused it"
    print "current time is: " + "%s" %(time.time())
    return

GuiRoot = Tk()
GuiRoot.title("Orbital Generator Client")

Label(GuiRoot).grid(row=1)
Label(GuiRoot,text="Set Quantumn Numbers",fg = "Black",font = "Times").grid(row=2)

Label(GuiRoot, text="n",justify = LEFT,padx = 20).grid(row=3)
Label(GuiRoot, text="l",justify = LEFT,padx = 20).grid(row=4)
Label(GuiRoot, text="m",justify = LEFT,padx = 20).grid(row=5)
Label(GuiRoot, text="Value",justify = LEFT,padx = 20).grid(row=6)
Qn = IntVar()
Ql = IntVar()
Qm = IntVar()
Qisovalue = DoubleVar()
Qn.set(4)
Ql.set(1)
Qm.set(0)
Qisovalue.set(0.5)
ent3 = Entry(GuiRoot,textvariable =Qn)
ent4 = Entry(GuiRoot,textvariable =Ql)
ent5 = Entry(GuiRoot,textvariable =Qm)
ent6 = Entry(GuiRoot,textvariable =Qisovalue)
ent3.grid(row=2, column=1)
ent4.grid(row=3, column=1)
ent5.grid(row=4, column=1)
ent6.grid(row=5, column=1)

Label(GuiRoot).grid(row=7)
Label(GuiRoot,text="Mesh Setting",fg = "Black",font = "Times").grid(row=8)

Label(GuiRoot, text="Grid Number of Each Dimension",justify = LEFT,padx = 20).grid(row=9)
Label(GuiRoot, text="Radius",justify = LEFT,padx = 20).grid(row=10)

Nx = IntVar()
Dn = IntVar()
Nx.set(51)
Dn.set(50)

ent9 = Entry(GuiRoot,textvariable =Nx)
ent10 = Entry(GuiRoot,textvariable =Dn)

ent9.grid(row=9, column=1)
ent10.grid(row=10, column=1)

OrbitalType = IntVar()
OrbitalType.set(1)

Label(GuiRoot).grid(row=11)
Label(GuiRoot, 
      text="""Choose your Orbitals Type:""",
      justify = LEFT,
      padx = 20).grid(row=12)

Radiobutton(GuiRoot, 
                text="Orbitals",
                padx = 20, 
                variable=OrbitalType, 
                value=1).grid(row=13,column =0)

Radiobutton(GuiRoot, 
                text="Orbitals real",
                padx = 20, 
                variable=OrbitalType, 
                value=2).grid(row=13,column =1)

ColorType = IntVar()
ColorType.set(1)
Label(GuiRoot, 
      text="""Draw color with phase:""",
      justify = LEFT,
      padx = 20).grid(row=14)

Radiobutton(GuiRoot, 
                text="With color",
                padx = 20, 
                variable=ColorType, 
                value=1).grid(row=15,column =0)

Radiobutton(GuiRoot, 
                text="Without color",
                padx = 20, 
                variable=ColorType, 
                value=2).grid(row=15,column =1)
def SendMes():
    print "current time is: " + "%s" % (time.time())
    Nx = int(ent9.get())
    Dn= int(ent10.get())
    
    n = int(ent3.get())
    l = int(ent4.get())
    m = int(ent5.get())
    isovalue = float(ent6.get())
    oType = OrbitalType.get ()
    if oType is 1:
        verts,faces,iso_phase = GetAbsWF(n,l,m,isovalue,Nx,Dn)
    else:
        verts,faces,iso_phase = GetRealWF(n,l,m,isovalue,Nx,Dn)
    if verts.size > 64000*3:
        ConsoleMess1 = Message(GuiRoot, text=time.ctime()).grid(row=19,column=0)
        ConsoleMess2 = Message(GuiRoot, text="too many grids\n").grid(row=19,column=1)
        return

    cType = ColorType.get ()
    if cType is 1:
        message = CreateIsoMessage(verts, faces, iso_phase,isovalue)
    else:
        message = CreateIsoMessageWithoutPhase(verts, faces)
    #message = CreateIsoMessage(verts, faces, iso_phase,isovalue)
    SendMessageBySocket(message)
    
    ConsoleMess1 = Message(GuiRoot, text=time.ctime()).grid(row=19,column=0)
    ConsoleMess2 = Message(GuiRoot, text="done\n").grid(row=19,column=1)
    return
Label(GuiRoot).grid(row=16)
SendCmd = Button(GuiRoot, text='Send Message', width=25, command=SendMes).grid(row=17)
Label(GuiRoot,text="Console",fg = "Black",font = "Times").grid(row=18)

GuiRoot.mainloop ()
