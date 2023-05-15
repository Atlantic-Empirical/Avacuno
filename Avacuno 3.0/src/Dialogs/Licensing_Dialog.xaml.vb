Partial Public Class Licensing_Dialog

    Private SL As cAvacunoLicensing
    Private ww As cWindowWrapper

    Public Sub New(ByRef nSL As cAvacunoLicensing)
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ww = New cWindowWrapper(Process.GetCurrentProcess.MainWindowHandle)
        SL = nSL
    End Sub

    Private Sub dlgLicensing_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Loaded
        UpdateLicensePane()

        Me.btnUpdateLicense.ToolTip = _
            "If you have purchased a license or an upgrade and have" & vbNewLine & _
            "received the confirmation email you can press this" & vbNewLine & _
            "button to complete the authorization process."

    End Sub

    Private Sub UpdateLicensePane()

        If SL.SkunkworksLicense = eSkunkworksLicense.UNLICENSED Then
            lblTitle.Content = "UNLICENSED"
            lblTrialTimeLeft.Content = ""
            lblKey.Content = ""
            lblStartDate.Content = ""
            lblEndDate.Content = ""
            lbFeatures.Items.Clear()
        Else
            lblTitle.Content = SL.ProductName & " v" & SL.ProductVersion & " " & SL.PackageName & IIf(SL.IsTrial, " TRIAL", "")
            lblTrialTimeLeft.Content = SL.TimeLeft
            lblKey.Content = SL.Key
            lblStartDate.Content = SL.StartDate.ToLongDateString

            If SL.TimeLeft = "Unlimited" Then
                lblEndDate.Content = ""
            Else
                lblEndDate.Content = SL.ExpireDate.ToLongDateString
            End If

            lbFeatures.Items.Clear()
            For Each f As String In SL.Features
                lbFeatures.Items.Add(f)
            Next
            gbOnlineActivation.IsEnabled = False
        End If
    End Sub

    Private Sub btnUpdateLicense_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdateLicense.Click
        Me.Topmost = False
        SL.RenewLicense(ww)
    End Sub

    Private Sub lblGetLicenseRequestFile_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles llGetLicenseRequestFile.Click
        Me.Topmost = False
        SL.GetLicenseRequestData(ww)
    End Sub

    Private Sub llSelectLiceseRequestFile_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles llSelectLiceseRequestFile.Click
        Me.Topmost = False
        SL.SelectLicenseRequestFile(ww)
    End Sub

    Private Sub llActivateProduct_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles llActivateProduct.Click
        Me.Topmost = False
        SL.ActivationDialog(ww)
    End Sub

    Private Sub btnDone_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDone.Click
        DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

End Class

Public Class cWindowWrapper
    Implements System.Windows.Forms.IWin32Window

    Public Sub New(ByVal handle As IntPtr)
        _hwnd = handle
    End Sub

    Public ReadOnly Property Handle() As IntPtr Implements System.Windows.Forms.IWin32Window.Handle
        Get
            Return _hwnd
        End Get
    End Property
    Private _hwnd As IntPtr

End Class
