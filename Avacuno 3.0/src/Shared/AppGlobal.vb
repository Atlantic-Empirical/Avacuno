Imports SMT.Multimedia.Enums

#Region "CLASSES"

<Serializable()> _
Public Class cWindowPositions

    Public Avacuno As System.Windows.Point
    Public DeckControl As System.Windows.Point
    Public FrameSplitting As System.Windows.Point
    Public Viewer As System.Windows.Point
    Public ViewerSize As eViewerSize
    Public FrameGrabbing As System.Windows.Point

    Public Sub New()
        Avacuno = New System.Windows.Point(0, 0)
        DeckControl = New System.Windows.Point(0, 0)
        FrameSplitting = New System.Windows.Point(0, 0)
        Viewer = New System.Windows.Point(0, 0)
        FrameGrabbing = New System.Windows.Point(0, 0)
    End Sub

    Public Sub New(ByRef AV As Avacuno_Form)
        Avacuno.X = AV.Left
        Avacuno.Y = AV.Top

        DeckControl.X = Avacuno.X
        DeckControl.Y = Avacuno.Y + AV.Height

        FrameSplitting.X = Avacuno.X
        FrameSplitting.Y = Avacuno.Y + AV.Height

        Viewer.X = Avacuno.X + AV.Width
        Viewer.Y = Avacuno.Y
        ViewerSize = eViewerSize.HD_Half_960x540

        FrameGrabbing.X = Avacuno.X
        FrameGrabbing.Y = Avacuno.Y + AV.Height

    End Sub

End Class

#End Region 'CLASSES

#Region "ENUMS"

Public Enum eInitializePlayerResults
    Success
    UnspecifiedFailure
    M2VNeedsProcessing
    FailedToBuildGraph
    FileFormatNotSupported
    NoMoreFilesInQueue
End Enum

Public Enum eAvacunoMode
    AppInit
    SysJP
    DTSAC3
    MPA
    PCM
    M2V
    MPG
    VOB
    SP
    L21
    AVI
    AVC
    VC1
    FrameSplitting
    YUV
    M2V_HD
    Ascent_Atls
    PacTV
    M2TS
    EVO
End Enum

Public Enum eFrameGrabType
    BMP
    JPEG
    GIF
    PNG
    TIF
    WMF
    YUV
End Enum

Public Enum eSkunkworksLicense
    UNLICENSED
    Trial
    BD
    DVD
End Enum

#End Region 'ENUMS
