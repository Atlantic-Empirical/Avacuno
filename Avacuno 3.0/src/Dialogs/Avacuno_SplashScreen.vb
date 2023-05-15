Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Drawing
Imports System.Windows.Forms

Public Class Avacuno_SplashScreen

    Private Splash As Bitmap

    Public Sub New(ByVal UseTimer As Boolean, ByRef nSplash As Bitmap)

        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        If Not UseTimer Then
            Me.ThreeSeconds.Stop()
        End If
        Splash = nSplash
        WriteVersionOnBitmap(Splash)
    End Sub

    Private Sub Avacuno_SplashScreen_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.M Then
            RunApp(True, True)
        Else
            RunApp(True, False)
        End If
    End Sub

    Private Sub Avacuno_SplashScreen_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        SelectBitmap(Splash)
    End Sub

    Private Sub Avacuno_SplashScreen_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        RunApp(True, False)
    End Sub

    Private Sub ThreeSeconds_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThreeSeconds.Tick
        RunApp(True, False)
    End Sub

    Private Sub RunApp(ByVal Closeout As Boolean, ByVal ForceMobile As Boolean)
        Try
            Me.ThreeSeconds.Stop()
            If ForceMobile Then
                Me.DialogResult = System.Windows.Forms.DialogResult.Yes
            Else
                Me.DialogResult = System.Windows.Forms.DialogResult.OK
            End If
        Catch ex As Exception
            Me.DialogResult = System.Windows.Forms.DialogResult.Abort
        Finally
            Me.Close()
        End Try
    End Sub

    Public Sub SelectBitmap(ByVal bitmap As Bitmap)
        ' Does this bitmap contain an alpha channel?
        If bitmap.PixelFormat <> PixelFormat.Format32bppArgb Then
            Throw New ApplicationException("The bitmap must be 32bpp with alpha-channel.")
        End If

        ' Get device contexts
        Dim screenDc As IntPtr = APIHelp.GetDC(IntPtr.Zero)
        Dim memDc As IntPtr = APIHelp.CreateCompatibleDC(screenDc)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim hOldBitmap As IntPtr = IntPtr.Zero

        Try
            ' Get handle to the new bitmap and select it into the current device context
            hBitmap = bitmap.GetHbitmap(Color.FromArgb(0))
            hOldBitmap = APIHelp.SelectObject(memDc, hBitmap)

            ' Set parameters for layered window update
            Dim newSize As New APIHelp.Size(bitmap.Width, bitmap.Height) ' Size window to match bitmap
            Dim sourceLocation As New APIHelp.Point(0, 0)
            Dim newLocation As New APIHelp.Point(Me.Left, Me.Top) ' Same as this window
            Dim blend As New APIHelp.BLENDFUNCTION()
            blend.BlendOp = APIHelp.AC_SRC_OVER ' Only works with a 32bpp bitmap
            blend.BlendFlags = 0 ' Always 0
            blend.SourceConstantAlpha = 255 ' Set to 255 for per-pixel alpha values
            blend.AlphaFormat = APIHelp.AC_SRC_ALPHA ' Only works when the bitmap contains an alpha channel
            ' Update the window
            Dim b As APIHelp.Bool = APIHelp.UpdateLayeredWindow(Handle, screenDc, newLocation, newSize, memDc, sourceLocation, 0, blend, APIHelp.ULW_ALPHA)
            'Debug.WriteLine(b)
        Catch ex As Exception
            Debug.WriteLine("Problem in SelectBitmap(). Error: " & ex.Message)
        Finally
            ' Release device context
            APIHelp.ReleaseDC(IntPtr.Zero, screenDc)
            If hBitmap <> IntPtr.Zero Then
                APIHelp.SelectObject(memDc, hOldBitmap)
                APIHelp.DeleteObject(hBitmap) ' Remove bitmap resources
            End If
            APIHelp.DeleteDC(memDc)
        End Try
    End Sub

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            ' Add the layered extended style (WS_EX_LAYERED) to this window
            Dim OutCreateParams As CreateParams = MyBase.CreateParams
            OutCreateParams.ExStyle = OutCreateParams.ExStyle Or APIHelp.WS_EX_LAYERED
            Return OutCreateParams
        End Get
    End Property

    Private Sub WriteVersionOnBitmap(ByRef BM As Bitmap)
        Try
            Dim str As String = "Stream Edition"
            str &= vbNewLine & "v" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
            Dim dc As Graphics = Graphics.FromImage(BM)
            'dc.Clear(BackColor)

            Dim drawFont As New Font("Arial", 9, FontStyle.Regular)

            Dim myBrush As New SolidBrush(Color.White)

            Dim drawFormat As New StringFormat()
            drawFormat.FormatFlags = StringFormatFlags.NoFontFallback
            drawFormat.Alignment = StringAlignment.Far
            drawFormat.LineAlignment = StringAlignment.Far

            ' Create point for upper-left corner of drawing.
            Dim x As Single = 482.0F
            Dim y As Single = 300.0F

            'Finally we apply the Drwaing on the form
            dc.DrawString(str, drawFont, myBrush, x, y, drawFormat)

        Catch ex As Exception
            Throw New Exception("Problem with WriteVersionOnBitmap(). Error: " & ex.Message, ex)
        End Try
    End Sub

End Class

Class APIHelp

    ' Required constants
    Public Const WS_EX_LAYERED As Int32 = &H80000
    Public Const HTCAPTION As Int32 = &H2
    Public Const WM_NCHITTEST As Int32 = &H84
    Public Const ULW_ALPHA As Int32 = &H2
    Public Const AC_SRC_OVER As Byte = &H0
    Public Const AC_SRC_ALPHA As Byte = &H1

    Public Enum Bool
        [False] = 0
        [True] = 1
    End Enum 'Bool

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure Point
        Public x As Int32
        Public y As Int32


        Public Sub New(ByVal x As Int32, ByVal y As Int32)
            Me.x = x
            Me.y = y
        End Sub 'New
    End Structure 'Point

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure Size
        Public cx As Int32
        Public cy As Int32

        Public Sub New(ByVal cx As Int32, ByVal cy As Int32)
            Me.cx = cx
            Me.cy = cy
        End Sub 'New
    End Structure 'Size

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Structure ARGB
        Public Blue As Byte
        Public Green As Byte
        Public Red As Byte
        Public Alpha As Byte
    End Structure 'ARGB

    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Public Structure BLENDFUNCTION
        Public BlendOp As Byte
        Public BlendFlags As Byte
        Public SourceConstantAlpha As Byte
        Public AlphaFormat As Byte
    End Structure 'BLENDFUNCTION

    Public Declare Auto Function UpdateLayeredWindow Lib "user32" Alias "UpdateLayeredWindow" (ByVal hwnd As IntPtr, ByVal hdcDst As IntPtr, ByRef pptDst As Point, ByRef psize As Size, ByVal hdcSrc As IntPtr, ByRef pprSrc As Point, ByVal crKey As Int32, ByRef pblend As BLENDFUNCTION, ByVal dwFlags As Int32) As Bool
    Public Declare Auto Function CreateCompatibleDC Lib "gdi32.dll" Alias "CreateCompatibleDC" (ByVal hDC As IntPtr) As IntPtr
    Public Declare Auto Function GetDC Lib "user32.dll" Alias "GetDC" (ByVal hWnd As IntPtr) As IntPtr
    Public Declare Auto Function ReleaseDC Lib "user32.dll" Alias "ReleaseDC" (ByVal hWnd As IntPtr, ByVal hDC As IntPtr) As Integer
    Public Declare Auto Function DeleteDC Lib "gdi32.dll" Alias "DeleteDC" (ByVal hdc As IntPtr) As Bool
    Public Declare Auto Function SelectObject Lib "gdi32.dll" Alias "SelectObject" (ByVal hDC As IntPtr, ByVal hObject As IntPtr) As IntPtr
    Public Declare Auto Function DeleteObject Lib "gdi32.dll" Alias "DeleteObject" (ByVal hObject As IntPtr) As Bool

End Class 'APIHelp
