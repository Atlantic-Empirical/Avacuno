Imports System.IO
Imports SMT.Multimedia.Formats.MPEG2
Imports SMT.Win.WinAPI.User32
Imports System.Windows.Forms

Partial Public Class M2VUserDataStripping_Dialog

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBrowse.Click
        Dim dlg As New OpenFileDialog
        dlg.Filter = "MPEG-2 Video (*.m2v, *.m2v_hd)|*.m2v;*.m2v_hd"
        dlg.FilterIndex = 0
        dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        dlg.Multiselect = False
        dlg.Title = "M2V Processing"
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtOrigStream.Text = dlg.FileName
            Me.txtNewFilename.Text = "new_" & Path.GetFileName(dlg.FileName)
        End If
    End Sub

    Private WithEvents SM2V As cSonyM2VProcessing

    Private Sub ProcessFile()
        Try
            Me.SM2V = New cSonyM2VProcessing
            Dim Source As String = Me.txtOrigStream.Text
            Dim Target As String = Path.GetDirectoryName(Source) & "\" & Me.txtNewFileName.Text
            If SM2V.RemoveSonyData(Source, Target) Then
                If MsgBox("Processing completed successfully." & vbNewLine & vbNewLine & HeadersRemoved & " Sony user data header(s) removed. Totaling " & (HeadersRemoved * 512) & " bytes", MsgBoxStyle.Information Or MsgBoxStyle.OkOnly, My.Settings.APPLICATION_NAME) = MsgBoxResult.Ok Then
                    Me.Close()
                End If
            End If
        Catch ex As Exception
            MsgBox("Problem with RemoveSonyData(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnProcess_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnProcess.Click
        Me.ProcessFile()
    End Sub

    Private Sub TickProgressBar(ByVal Cur As Byte) Handles SM2V.PercentTick
        'Debug.WriteLine("% tick: " & Cur)
        If Cur < 0 Or Cur > 100 Then Exit Sub
        Me.pbMain.Value = Cur
    End Sub

    Private HeadersRemoved As Integer = 0
    Private Sub HeaderCountTick() Handles SM2V.SonyHeaderFound
        HeadersRemoved += 1
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        If Not SM2V Is Nothing Then SM2V.CancelProcssing = True
        Me.Close()
    End Sub

    Public Property FilePath() As String
        Get
            Return Me.txtOrigStream.Text
        End Get
        Set(ByVal value As String)
            Me.txtOrigStream.Text = value
            Me.txtNewFileName.Text = "new_" & Path.GetFileName(value)
        End Set
    End Property

    Private Sub SonyM2V_Dialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Me.btnBrowse.Focus()
    End Sub

End Class
