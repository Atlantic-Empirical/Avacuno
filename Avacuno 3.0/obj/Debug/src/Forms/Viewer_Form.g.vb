﻿#ExternalChecksum("..\..\..\..\src\Forms\Viewer_Form.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","8404926964A69DA56E236AC6793A7E7A")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:2.0.50727.3053
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes


'''<summary>
'''Viewer_Form
'''</summary>
<Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>  _
Partial Public Class Viewer_Form
    Inherits System.Windows.Window
    Implements System.Windows.Markup.IComponentConnector
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",5)
    Friend WithEvents bdForm As System.Windows.Controls.Border
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",20)
    Friend WithEvents dpLayoutRoot As System.Windows.Controls.DockPanel
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",21)
    Friend WithEvents bdTitleBar As System.Windows.Controls.Border
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",23)
    Friend WithEvents cvTitleBar As System.Windows.Controls.Canvas
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",28)
    Friend WithEvents rtTitleBar As System.Windows.Shapes.Rectangle
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",41)
    Friend WithEvents imgIcon As System.Windows.Controls.Image
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",44)
    Friend WithEvents txtCaption As System.Windows.Controls.TextBlock
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",48)
    Friend WithEvents bdViewer As System.Windows.Controls.Border
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",49)
    Friend WithEvents cvViewer As System.Windows.Controls.Canvas
    
    #End ExternalSource
    
    
    #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",50)
    Friend WithEvents bdVidWin As System.Windows.Controls.Border
    
    #End ExternalSource
    
    Private _contentLoaded As Boolean
    
    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute()>  _
    Public Sub InitializeComponent() Implements System.Windows.Markup.IComponentConnector.InitializeComponent
        If _contentLoaded Then
            Return
        End If
        _contentLoaded = true
        Dim resourceLocater As System.Uri = New System.Uri("/Avacuno;component/src/forms/viewer_form.xaml", System.UriKind.Relative)
        
        #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",1)
        System.Windows.Application.LoadComponent(Me, resourceLocater)
        
        #End ExternalSource
    End Sub
    
    <System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never),  _
     System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")>  _
    Sub System_Windows_Markup_IComponentConnector_Connect(ByVal connectionId As Integer, ByVal target As Object) Implements System.Windows.Markup.IComponentConnector.Connect
        If (connectionId = 1) Then
            Me.bdForm = CType(target,System.Windows.Controls.Border)
            Return
        End If
        If (connectionId = 2) Then
            Me.dpLayoutRoot = CType(target,System.Windows.Controls.DockPanel)
            Return
        End If
        If (connectionId = 3) Then
            Me.bdTitleBar = CType(target,System.Windows.Controls.Border)
            Return
        End If
        If (connectionId = 4) Then
            Me.cvTitleBar = CType(target,System.Windows.Controls.Canvas)
            Return
        End If
        If (connectionId = 5) Then
            Me.rtTitleBar = CType(target,System.Windows.Shapes.Rectangle)
            
            #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",28)
            AddHandler Me.rtTitleBar.MouseLeftButtonDown, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.OnDragMoveWindow)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 6) Then
            Me.imgIcon = CType(target,System.Windows.Controls.Image)
            
            #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",41)
            AddHandler Me.imgIcon.MouseLeftButtonDown, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.OnDragMoveWindow)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 7) Then
            Me.txtCaption = CType(target,System.Windows.Controls.TextBlock)
            
            #ExternalSource("..\..\..\..\src\Forms\Viewer_Form.xaml",44)
            AddHandler Me.txtCaption.MouseLeftButtonDown, New System.Windows.Input.MouseButtonEventHandler(AddressOf Me.OnDragMoveWindow)
            
            #End ExternalSource
            Return
        End If
        If (connectionId = 8) Then
            Me.bdViewer = CType(target,System.Windows.Controls.Border)
            Return
        End If
        If (connectionId = 9) Then
            Me.cvViewer = CType(target,System.Windows.Controls.Canvas)
            Return
        End If
        If (connectionId = 10) Then
            Me.bdVidWin = CType(target,System.Windows.Controls.Border)
            Return
        End If
        Me._contentLoaded = true
    End Sub
End Class