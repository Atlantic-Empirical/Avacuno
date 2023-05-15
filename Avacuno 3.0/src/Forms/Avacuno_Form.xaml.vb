#Region "IMPORTS"

Imports SMT.Win
Imports SMT.Win.Filesystem
Imports SMT.IPProtection.SafeNet
Imports SMT.IPProtection.SafeNet.Avacuno
Imports SMT.Multimedia.Players
Imports SMT.Multimedia.Players.AVC
Imports SMT.Multimedia.Players.AVI
Imports SMT.Multimedia.Players.DTSAC3Player
Imports SMT.Multimedia.Players.FrameSplitter
Imports SMT.Multimedia.Players.M2V
Imports SMT.Multimedia.Players.M2TS
Imports SMT.Multimedia.Players.MPA
Imports SMT.Multimedia.Players.M1V
Imports SMT.Multimedia.Players.MPG
Imports SMT.Multimedia.Players.MOV
Imports SMT.Multimedia.Players.PCMWAVPlayer
Imports SMT.Multimedia.Players.VC1
Imports SMT.Multimedia.Players.VOB
Imports SMT.Multimedia.Players.YUV
Imports System.Drawing
Imports System.IO
Imports Microsoft.Win32
Imports System.Windows.Interop
Imports SMT.Multimedia.Enums

#End Region 'IMPORTS

Class Avacuno_Form

#Region "PROPERTIES"

    Protected Friend WithEvents PLAYER As cBasePlayer

    Public ReadOnly Property SplashBitmap() As Bitmap
        Get
            Dim str As Stream = Me.GetType.Module.Assembly.GetManifestResourceStream("SMT.Applications.Avacuno.Avacuno_Splash_071101.png")
            Return New Bitmap(str)
        End Get
    End Property

    Private ReadOnly Property Handle() As IntPtr
        Get
            If _Handle = Nothing Then
                _Handle = New WindowInteropHelper(Me).Handle
            End If
            Return _Handle
        End Get
    End Property
    Private _Handle As IntPtr

    Private Mode As eAvacunoMode
    Private ForceVMR9 As Boolean = True

#End Region 'PROPERTIES

#Region "CONSTRUCTOR"

#End Region 'CONSTRUCTOR

#Region "UI EVENTS"

#Region "UI:FORM"

#End Region 'UI:FORM

#Region "UI:CONTROLS"

#Region "UI:CONTROLS:FILE SELECTION"

    Private Sub btnAddFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAddFile.Click
        FileBrowse()
    End Sub

    Private Sub btnRemoveFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRemoveFile.Click
        RemoveSelectedFile()
    End Sub

    Private Sub btnClearFileList_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnClearFileList.Click
        ClearFileList()
    End Sub

#End Region 'UI:CONTROLS:FILE SELECTION

#Region "UI:CONTROLS:TRANSPORT"

    Private Sub btnPlayPause_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPlayPause.Click

    End Sub

    Private Sub btnStop_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStop.Click

    End Sub

    Private Sub btnRewind_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRewind.Click

    End Sub

    Private Sub btnFastForward_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFastForward.Click

    End Sub

    Private Sub btnRestartStream_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnRestartStream.Click

    End Sub

    Private Sub btnJumpBack_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnJumpBack.Click

    End Sub

    Private Sub btnFrameStep_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnFrameStep.Click

    End Sub

#End Region 'UI:CONTROLS:TRANSPORT

#End Region 'UI:CONTROLS

#End Region 'UI EVENTS

#Region "FUNCTIONALITY"

#Region "FUNCTIONALITY:FILE MANAGEMENT"

    Private Sub FileBrowse()
        Try
            Dim OFD As New OpenFileDialog
            OFD.Filter = Me.GetFileBrowseFilter
            OFD.FilterIndex = 0
            If My.Settings.LAST_FOLDER = "" Then My.Settings.LAST_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            OFD.InitialDirectory = My.Settings.LAST_FOLDER
            OFD.Multiselect = False
            OFD.Title = "Select File"
            If OFD.ShowDialog = True Then
                AddFile(OFD.FileName)
                My.Settings.LAST_FOLDER = Path.GetDirectoryName(OFD.FileName)
                My.Settings.Save()
            End If
        Catch ex As Exception
            Me.AddConsoleLine("Problem with FileBrowse(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub AddFile(ByVal FilePath As String)
        Try
            Me.lbFiles.Items.Add(New cFilePath(FilePath))
            SetAddRemoveFileButtonState()
        Catch ex As Exception
            Me.AddConsoleLine("Problem with AddFile(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub RemoveSelectedFile()
        If Me.lbFiles.SelectedItem Is Nothing Then Exit Sub
        Me.lbFiles.Items.RemoveAt(Me.lbFiles.SelectedIndex)
        SetAddRemoveFileButtonState()
    End Sub

    Private Sub ClearFileList()
        Me.lbFiles.Items.Clear()
    End Sub

    Private Sub SetAddRemoveFileButtonState()
        Try

            'Dim ESs As Byte = 0
            'Dim PSs As Byte = 0

            'For Each F As cFilePath In Me.lbFiles.Items
            '    If F.IsMultimediaElementaryStream Then ESs += 1
            '    If F.IsMultimediaProgramStream Or F.IsMultimediaTransportStream Then PSs += 1
            'Next

            'Me.btnAddFile.IsEnabled = True
            'If ESs = 2 Then Me.btnAddFile.IsEnabled = False
            'If PSs = 1 Then Me.btnAddFile.IsEnabled = False

            Me.btnRemoveFile.IsEnabled = Me.lbFiles.Items.Count > 0

        Catch ex As Exception
            Me.AddConsoleLine("Problem with SetAddRemoveFileButtonState(). Error: " & ex.Message)
        End Try
    End Sub

    Private Function GetFileBrowseFilter() As String
        Try
            Dim VESs As Byte = 0
            Dim AESs As Byte = 0
            Dim PSs As Byte = 0
            Dim TSs As Byte = 0

            For Each F As cFilePath In Me.lbFiles.Items
                If F.IsMultimediaVideoElementaryStream Then VESs += 1
                If F.IsMultimediaAudioElementaryStream Then AESs += 1
                If F.IsMultimediaProgramStream Then PSs += 1
                If F.IsMultimediaTransportStream Then TSs += 1
            Next

            Dim li As New List(Of String)

            'ONLY ALLOW AUDIO ES WHEN NO TS/PS IS SELECTED
            'ALLOW ONLY ONE AES
            If PSs = 0 And TSs = 0 And AESs < 1 Then
                li.Add("Dolby AC3 (*.AC3)|*.ac3" _
                            & "|DTS (*.DTS)|*.dts" _
                            & "|PCM (*.PCM, *.WAV)|*.pcm;*.wav")
                '& "|MPEG Audio (*.mpa)|*.mpa" _
            End If

            '    For splitting a PS/TS with VES         For splitting VES with VES or queuing
            If ((PSs = 1 Or TSs = 1) And VESs = 0) Or ((PSs = 0 And TSs = 0)) Then
                li.Add("MPEG Elementary Video Streams (*.M2V, *.M1V)|*.m2v;*.m1v" _
                            & "|AVC/h264 (*.AVC, *.264, *.H264)|*.avc;*.264;*.h264" _
                            & "|YUV i420 (*.I420, *.IYUV)|*.i420;*.iYUV" _
                            & "|VC1 (*.VC1)|*.vc1")
            End If

            If VESs < 2 Then
                li.Add("MPEG-2 Transport Stream (*.M2T,*.M2TS)|*.atls;*.m2t;*.m2ts" _
                            & "|MPEG Program Stream (*.MPG, *.MPEG)|*.mpg;*.mpeg" _
                            & "|DVD-Video VOB (*.VOB)|*.vob" _
                            & "|Quicktime MOV (*.MOV)|*.mov" _
                            & "|Audio-Video Interleave (*.AVI)|*.avi")
            End If

            li.Add("All Applicable Types|" & GetSupportedFormats(li))

            Dim out As New System.Text.StringBuilder
            For i As Integer = li.Count - 1 To 0 Step -1
                out.Append(If(i = li.Count - 1, "", "|") & li(i))
            Next

            Dim out2 As String = out.ToString
            Return out2
        Catch ex As Exception
            Me.AddConsoleLine("Problem with GetFileBrowseFilter(). Error: " & ex.Message)
            Return ""
        End Try
    End Function

    Private Function GetSupportedFormats(ByVal li As List(Of String)) As String
        Try
            Dim out As New System.Text.StringBuilder
            Dim s1() As String
            For Each s As String In li
                s1 = Split(s, "|")
                For i As Integer = 1 To UBound(s1) Step 2
                    out.Append(s1(i) & ";")
                Next
            Next
            Return out.ToString.TrimEnd(New Char() {";"})
        Catch ex As Exception
            Me.AddConsoleLine("Problem with GetSupportedFormats(). Error: " & ex.Message)
            Return ""
        End Try
    End Function

#End Region 'FUNCTIONALITY:FILE MANAGEMENT

#End Region 'FUNCTIONALITY

#Region "PLAYER"

#Region "PLAYER:SETUP"

    Private Function LoadContent() As Boolean
        Try
            '' VERIFY ALL FILES EXIST
            'If VideoAPath <> "" AndAlso Not File.Exists(VideoAPath) Then
            '    MessageBox.Show("VA file does not exist." & vbNewLine & VideoAPath, My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    txtVideo_A.Text = ""
            '    Exit Function
            'End If
            'If VideoBPath <> "" AndAlso Not File.Exists(VideoBPath) Then
            '    MessageBox.Show("VB file does not exist." & vbNewLine & VideoBPath, My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    txtVideo_B.Text = ""
            '    Exit Function
            'End If
            'If AudioPath <> "" AndAlso Not File.Exists(AudioPath) Then
            '    MessageBox.Show("Audio file does not exist." & vbNewLine & AudioPath, My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '    txtAudio.Text = ""
            '    Exit Function
            'End If

            Me.Cursor = Cursors.Wait

            Dim res As eInitializePlayerResults = InitializePlayer()
            Select Case res
                Case eInitializePlayerResults.UnspecifiedFailure
                    Me.Cursor = Cursors.Arrow
                    Throw New Exception("InitializePlayer() Failed.")
                Case eInitializePlayerResults.M2VNeedsProcessing
                    Me.Cursor = Cursors.Arrow
                    Exit Function
                Case eInitializePlayerResults.FailedToBuildGraph
                    MessageBox.Show("Avacuno was not able to build graph for selected file(s).", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Me.Cursor = Cursors.Arrow
                    Return False
                Case eInitializePlayerResults.FileFormatNotSupported
                    MessageBox.Show("The selected file is in an unsupported format.", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
                    Me.Cursor = Cursors.Arrow
                    Return False
                Case eInitializePlayerResults.Success
                    Me.cvTransport.IsEnabled = True
                    Me.bdFileSelection.IsEnabled = False
                    Me.Cursor = Cursors.Arrow
            End Select
            Return True
        Catch ex As Exception
            Me.AddConsoleLine("Problem with LoadContent(). Error: " & ex.Message)
            Return False
        End Try
    End Function

    Public Function InitializePlayer() As eInitializePlayerResults
        Try
            If Not PLAYER Is Nothing Then
                PLAYER.Dispose()
                PLAYER = Nothing
            End If

            Dim SingVidPath As String = ""

            'If Me.InFileQueueMode Then
            '    If CurrentQueueIndex > UBound(FileQueue) Then Return eInitializePlayerResults.NoMoreFilesInQueue
            '    SingVidPath = Me.FileQueue(CurrentQueueIndex).FullPath 'this needs real logic to determine what the next file should be, if any
            '    CurrentQueueIndex += 1
            'Else
            '    ' CHECK FOR FRAME SPLITTING
            '    If txtVideo_A.Text <> "" And txtVideo_B.Text <> "" Then
            '        Dim ext As String = Path.GetExtension(Me.Video_A_FileName)
            '        Select Case ext.ToLower
            '            Case ".yuv", ".iyuv", ".i420"
            '                MessageBox.Show("YUV file must be B stream for framesplitting.", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
            '                Return eInitializePlayerResults.FailedToBuildGraph
            '        End Select
            '        PLAYER = New cFrameSplittingPlayer
            '        If Not FSPlayer.BuildFrameSplittingGraph(txtVideo_A.Text, txtVideo_B.Text, Me.Handle) Then Return eInitializePlayerResults.FailedToBuildGraph
            '        Mode = eAvacunoMode.FrameSplitting
            '        PLAYER = FSPlayer
            '        Me.TimesearchEnable(True)
            '        ShuttleEnable(False)
            '        GoTo AudioCheck
            '    End If

            '    ' CHECK FOR AUDIO ONLY
            '    If txtVideo_A.Text = "" And txtVideo_B.Text = "" And txtAudio.Text <> "" Then
            '        'currently not supported
            '        Me.TimesearchEnable(False)
            '        ShuttleEnable(False)
            '        Return eInitializePlayerResults.UnspecifiedFailure
            '    End If

            '    ' CHECK FOR SINGLE VIDEO
            '    If Me.txtVideo_A.Text <> "" Then
            '        SingVidPath = Me.txtVideo_A.Text
            '    ElseIf txtVideo_B.Text <> "" Then
            '        SingVidPath = txtVideo_B.Text
            '    End If

            'End If

            Select Case Path.GetExtension(SingVidPath).ToLower
                'Case ".vc1"
                '    PLAYER = New cVC1Player_MC
                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    Mode = eAvacunoMode.VC1
                '    Me.TimesearchEnable(True)
                '    ShuttleEnable(True)
                '    GoTo AudioCheck

                'Case ".avc", ".h264", ".264"
                '    PLAYER = New cAVCPlayer
                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    Mode = eAvacunoMode.AVC
                '    Me.TimesearchEnable(True)
                '    ShuttleEnable(True)
                '    GoTo AudioCheck

                'Case ".m2v_hd"
                '    PLAYER = New cHDMPEG2Player
                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    Mode = eAvacunoMode.M2V_HD
                '    Me.TimesearchEnable(True)
                '    ShuttleEnable(True)
                '    GoTo AudioCheck

                'Case ".m2v"
                '    'If CheckForSonyData(SingVidPath) Then Return eInitializePlayerResults.M2VNeedsProcessing
                '    PLAYER = New cSDMPEG2Player
                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    Mode = eAvacunoMode.M2V
                '    Me.TimesearchEnable(False)
                '    ShuttleEnable(False)
                '    GoTo AudioCheck

                'Case ".i420", ".iyuv"
                '    PLAYER = New cYUVPlayer

                '    'NEED TO KNOW WHAT FR THE USER WANTS THE YUV PLAYED AT
                '    Dim s As String = InputBox("YUV Target Frame Rate", "Avacuno YUV Playback", "")
                '    Try
                '        If Not IsNumeric(s) Then Throw New Exception("Non-numeric framerate supplied by user")
                '        Dim FR As Double = CType(s, Double)
                '        Select Case FR
                '            Case 23.976, 24, 25, 29.97, 30
                '                'we're cool
                '            Case Else
                '                Throw New Exception("Non-supported framerate supplied by user.")
                '        End Select
                '        If Not PLAYER.SetPlayerProperty(0, Math.Round(10000000 / FR, 0), "") Then Return eInitializePlayerResults.UnspecifiedFailure
                '    Catch ex As Exception
                '        MessageBox.Show("Invalid YUV Framerate", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Exclamation)
                '        PLAYER.Dispose()
                '        PLAYER = Nothing
                '        Return eInitializePlayerResults.UnspecifiedFailure
                '    End Try

                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    Mode = eAvacunoMode.YUV
                '    Me.TimesearchEnable(True)
                '    ShuttleEnable(False)
                '    GoTo AudioCheck

                'Case ".atls", ".m2t", ".m2ts"

                '    Dim pw As String = InputBox("Atlas playback password:", "PW", "")
                '    If pw <> "smtavacuno" Then Process.GetCurrentProcess.Kill()

                '    PLAYER = New cM2TSPlayer
                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    'Dim s() As cAscentAudioPinInfo = CType(PLAYER, cAscentTXPlayer).GetAudioStreamPIDs
                '    'If s.Length > 1 Then
                '    '    Dim dlg As New AtlasAudioSelector_Dialog(Me, s)
                '    '    If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
                '    '        Dim selectedpin As cAscentAudioPinInfo = dlg.SelectedPin
                '    '        If Not CType(PLAYER, cAscentTXPlayer).SetAudioStream(selectedpin) Then
                '    '            Throw New Exception("Problem setting audio stream for Atlas playback.")
                '    '        End If
                '    '    End If
                '    'Else
                '    '    If Not CType(PLAYER, cAscentTXPlayer).SetAudioStream(s(0)) Then
                '    '        Throw New Exception("Problem setting audio stream for Atlas playback.")
                '    '    End If
                '    'End If
                '    Mode = eAvacunoMode.Ascent_Atls
                '    TimesearchEnable(True)
                '    ShuttleEnable(True)
                '    GoTo ViewerSetup

                'Case ".avi"
                '    PLAYER = New cAVIPlayer
                '    If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '    Mode = eAvacunoMode.AVI
                '    Me.TimesearchEnable(True)
                '    ShuttleEnable(True)
                '    Return eInitializePlayerResults.Success

                'Case Else
                '    If InStr(Path.GetFileName(SingVidPath).ToLower, "pactv") Then
                '        PLAYER = New cPacTVPlayer
                '        If Not PLAYER.BuildGraph(SingVidPath, Me.Handle, Me.ForceVMR9, Me) Then Return eInitializePlayerResults.FailedToBuildGraph
                '        Mode = eAvacunoMode.PacTV
                '        Me.TimesearchEnable(True)
                '        ShuttleEnable(True)
                '        GoTo ViewerSetup
                '    Else
                '        Return eInitializePlayerResults.FileFormatNotSupported
                '    End If

            End Select

            'AudioCheck:
            '            If txtAudio.Text <> "" Then
            '                If Not PLAYER.Graph.AddAudioToGraph(txtAudio.Text) Then Throw New Exception("Unable to add audio to graph.")
            '            End If

            'ViewerSetup:
            '            If Not Me.PLAYER.Viewer Is Nothing Then
            '                PLAYER.ShowViewer("Avacuno Viewer")
            '                PLAYER.Viewer.Icon = Me.Icon
            '                PLAYER.Viewer.Location = Me.WindowPositions.Viewer
            '                CType(PLAYER.Viewer, SMT.Common.Viewer_Form).ViewerSize = Me.WindowPositions.ViewerSize
            '                'CType(Player.Viewer, SMT.Common.Viewer_Form).ViewerSize = eViewerSize.QuarterHD
            '            End If

            '            If Mode = eAvacunoMode.FrameSplitting Then
            '                Dim StreamAProperties As cSourceProperties = FSPlayer.GetSourceProperties(True)
            '                Dim StreamBProperties As cSourceProperties = FSPlayer.GetSourceProperties(False)
            '                If Not StreamAProperties.FrameRate = StreamBProperties.FrameRate Then
            '                    Dim s As String = "WARNING: The framerate of the files selected for splitting do not match. Stuttering will occur."
            '                    s &= vbNewLine & vbNewLine
            '                    s &= Me.Video_A_FileName & " = " & StreamAProperties.FrameRate & "fps"
            '                    s &= vbNewLine
            '                    s &= Me.Video_B_Filename & " = " & StreamBProperties.FrameRate & "fps"
            '                    s &= vbNewLine & vbNewLine
            '                    s &= "NOTE: 3:2 is not applied in frame splitting mode."
            '                    MessageBox.Show(s, My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Warning)
            '                End If
            '                Me.lblSourceFrameRate.Text = StreamAProperties.FrameRate
            '                Me.lblImageHeight.Text = StreamAProperties.Height
            '            Else
            '                Dim PictureProperties As cSourceProperties = PLAYER.GetSourceProperties
            '                Me.lblSourceFrameRate.Text = PictureProperties.FrameRate
            '                Me.lblImageHeight.Text = PictureProperties.Height
            '            End If

            Return eInitializePlayerResults.Success
        Catch ex As Exception
            Throw New Exception("Problem with InitalizePlayer(). Error: " & ex.Message, ex)
        End Try
    End Function

#End Region 'PLAYER:SETUP

#Region "PLAYER:TRANSPORT"

    '    Public Sub ShuttleEnable(ByVal Enable As Boolean)
    '        Me.btnFastForward.Enabled = Enable
    '        Me.btnRewind.Enabled = Enable
    '    End Sub

    '    Public Sub TimesearchEnable(ByVal Enable As Boolean)
    '        Me.gbTimesearch.Enabled = Enable
    '        Me.tbPlayPosition.Enabled = Enable
    '        Me.btnRestartStream.Enabled = Enable
    '        Me.btnJumpBack.Enabled = Enable
    '    End Sub

    '    Public Sub PlayPause()
    '        Try
    '            If Me.Mode = eAvacunoMode.SysJP OrElse PLAYER Is Nothing Then
    '                Me.Cursor = Cursors.Wait
    '                If Me.InFileQueueMode Then
    '                    Dim Res As eInitializePlayerResults = Me.InitializePlayer
    '                    If Res = eInitializePlayerResults.Success Then
    '                        Me.Cursor = Cursors.Arrow
    '                        Me.gbTransport.Enabled = True
    '                        Me.bdFileSelection.IsEnabled = False
    '                        GoTo StartPlayback
    '                    ElseIf Res = eInitializePlayerResults.NoMoreFilesInQueue Then
    '                        MessageBox.Show("End of file queue reached.", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information)
    '                    End If
    '                    FileQueue = Nothing
    '                    CurrentQueueIndex = 0
    '                    Me.StopPlayback()
    '                    Exit Sub
    '                Else
    '                    If LoadContent() Then
    '                        Me.Cursor = Cursors.Arrow
    '                        GoTo StartPlayback
    '                    Else
    '                        Me.ShowJacketPicture()
    '                    End If
    '                End If
    '            Else
    'StartPlayback:
    '                Select Case PLAYER.PlayState
    '                    Case ePlayState.Playing
    '                        ScrollBarTicker.Stop()
    '                        PLAYER.Pause()
    '                        Me.btnPlayPause.Text = "PLAY"

    '                    Case Else
    '                        'MsgBox("Player.Play().")
    '                        PLAYER.Play()
    '                        ScrollBarTicker.Start()
    '                        Me.btnPlayPause.Text = "PAUSE"
    '                        Me.tbPlayPosition.Maximum = PLAYER.Duration_InReferenceTime / 10000000
    '                        Me.tbPlayPosition.TickFrequency = Me.tbPlayPosition.Maximum / 100
    '                        'Debug.WriteLine("Duration=" & Player.Duration_InReferenceTime)
    '                        Me.lblDuration.Text = New cTimecode(PLAYER.Duration_InReferenceTime, Me.StreamAProperties.ATPF, DoubleTo_eFrameRate(Me.StreamAProperties.FrameRate)).ToString

    '                End Select
    '            End If
    '        Catch ex As Exception
    '            MsgBox(ex.Message)
    '            AddConsoleLine("Problem with StartPlayback. Error: " & ex.Message)
    '        End Try
    '    End Sub

    '    Private Function DoubleTo_eFrameRate(ByVal FR As Double) As eFramerate
    '        Select Case FR
    '            Case 23.976, 24
    '                Return eFramerate.FILM
    '            Case 25
    '                Return eFramerate.PAL
    '            Case 29.97, 30
    '                Return eFramerate.NTSC
    '            Case Else
    '                Return Nothing
    '        End Select
    '    End Function

    '    Public Sub StopPlayback()
    '        Try
    '            Me.Cursor = Cursors.Wait
    '            ScrollBarTicker.Stop()
    '            If Not PLAYER Is Nothing Then
    '                PLAYER.Stop()
    '                PLAYER.Dispose()
    '                PLAYER = Nothing
    '            End If
    '            Me.ShowJacketPicture()
    '            Me.lblCurrentSpeed.Text = ""
    '            Me.lblPosition.Text = "00:00:00;00"
    '            Me.lblDuration.Text = "00:00:00;00"
    '            Me.lblSourceTimecode.Text = "00:00:00;00"
    '            Me.btnPlayPause.Text = "PLAY"
    '            Me.bdFileSelection.IsEnabled = True
    '            Me.cvTransport.IsEnabled = False
    '            txtVideo_A.Text = ""
    '            txtVideo_B.Text = ""
    '            txtAudio.Text = ""
    '            Me.lblSourceFrameRate.Text = ""
    '            Me.lblImageHeight.Text = ""
    '            Me.Cursor = Cursors.Default
    '            Me.tbPlayPosition.Value = 0
    '            Me.lbFileQueue.Items.Clear()
    '        Catch ex As Exception
    '            AddConsoleLine("Problem with fileplayback-stop. Error: " & ex.Message)
    '        End Try
    '    End Sub

    '    Public Sub FastForward()
    '        Try
    '            PLAYER.FastForward()
    '            Me.btnPlayPause.Text = "PLAY"
    '        Catch ex As Exception
    '            AddConsoleLine("Problem with FastForward(). Error: " & ex.Message)
    '        End Try
    '    End Sub

    '    Public Sub Rewind()
    '        Try
    '            PLAYER.Rewind()
    '            Me.btnPlayPause.Text = "PLAY"
    '        Catch ex As Exception
    '            Me.AddConsoleLine("Problem with Rewind(). Error: " & ex.Message)
    '        End Try
    '    End Sub

    '    Public Sub FrameStep()
    '        Try
    '            PLAYER.FrameStep()
    '            btnPlayPause.Text = "PLAY"
    '        Catch ex As Exception
    '            Me.AddConsoleLine("Problem with FrameStep(). Error: " & ex.Message)
    '        End Try
    '    End Sub

#End Region 'PLAYER:TRANSPORT

#Region "PLAYER:EVENTS"

    Private Sub StreamEnd() Handles Player.evStreamEnd
        'If Me.InFileQueueMode Then
        '    'start the next file in queue
        '    Player.Stop()
        '    Player.Dispose()
        '    Player = Nothing
        '    PlayPause()
        'Else
        '    StopPlayback()
        'End If
    End Sub

    Private Sub Handle_FilterCheckFailure(ByVal Msg As String) Handles Player.evFilterCheckFailure
        'MsgBox(Msg)
        'Player.Dispose()
        'Player = Nothing
        'Me.ShowJacketPicture()
    End Sub

#End Region 'PLAYER:EVENTS

#End Region 'PLAYER

#Region "UTILITY"

    'Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
    '    Try
    '        'Player.SetTimeFormat()

    '        'RenderJacketPic()
    '        'Debug.WriteLine(CType(Player, cJacketPicturePlayer_HD).RenderImageResource(Me.SysJPBitmap, 0, 0))

    '        'Dim i As OABool
    '        'HR = Me.VideoWin.get_FullScreenMode(i)
    '        'HR = Me.VideoWin.put_FullScreenMode(OABool.True)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)
    '        'HR = VMRMonitorConfig.SetMonitor(1)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)

    '        'HR = Me.iKeystoneMixer_hd.HighContrastSP(1)
    '        'Debug.WriteLine(HR)


    '        'Dim i As Integer = 0
    '        'HR = Me.iDLIO.GetIOFeatures(i)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)
    '        'If (i And SMT.Common.Media.DirectShow.BlackMagic.eIOFeatures.DECKLINK_IOFEATURES_HASCOMPOSITEVIDEOOUTPUT) Then
    '        '    MsgBox("hi")
    '        'End If

    '        'Dim fs As New FileStream("G:\Media\DV\x.avi", IO.FileMode.Open)
    '        'fs.WriteByte(1)
    '        'fs.Close()
    '        'fs.Dispose()
    '        'fs = Nothing
    '        ''File.Delete("G:\Media\DV\x.avi")

    '        'MCE_AVC_IMC = CType(MCE_AVC, IModuleConfig)
    '        'HR = MCE_AVC_IMC.SetValue(New Guid("85c6cbac-fbed-f244-a07c-6f9abd799e64"), Nothing)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)

    '        'Dim ca As SeekingCapabilities
    '        'HR = seek.GetCapabilities(ca)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)

    '        'Dim d As Double
    '        'HR = seek.GetRate(d)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)
    '        'Debug.WriteLine(d)

    '        'VTR.SendCmd(cDeckLink_VTR.eCMDs.Play)

    '        'Dim i As Integer
    '        'HR = Player.Graph.MediaCtrl.GetState(1000, i)
    '        'If HR < 0 Then Marshal.ThrowExceptionForHR(HR)


    '        'Me.VideoDecoder_PP.GetPages(PGs)
    '        'Dim FI As New FilterInfo
    '        'DsUtils.OleCreatePropertyFrame(Me.Handle, 0, 0, FI.achName, 1, Me.VSDecoder, PGs.cElems, PGs.pElems, 0, 0, Nothing)

    '        'Me.MCE_AVC_PP.GetPages(PGs)
    '        'Dim FI As New FilterInfo
    '        'DsUtils.OleCreatePropertyFrame(Me.Handle, 0, 0, FI.achName, 1, Me.MCE_AVC, PGs.cElems, PGs.pElems, 0, 0, Nothing)
    '    Catch ex As Exception
    '        AddConsoleLine("problem getting video ren properties. error: " & ex.Message)
    '    End Try

    '    'Dim i As Integer
    '    'nvAudioAtts.GetLong(nvcommon.EINvidiaAudioDecoderProps.NVAUDDEC_STATS, nvcommon.ENvidiaAudioDecoderProps_Stats.NVAUDDEC_STATS_BITRATE, i)
    '    ''nvAudioAtts.GetLong(nvcommon.EINvidiaAudioDecoderProps.NVAUDDEC_STATS, nvcommon.ENvidiaAudioDecoderProps_Stats.NVAUDDEC_STATS_BITRATE, i)
    '    ''Debug.WriteLine("rate: " & i)
    '    'Debug.WriteLine("Audio bitrate: " & (i / 1024) & "k")
    'End Sub

    Public Sub AddConsoleLine(ByVal msg As String)
        MessageBox.Show(msg, My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Information)
    End Sub

#End Region 'UTILITY

#Region "OLD AVACUNO"

    '#Region "PROPERTIES"

    '    Public WithEvents Player As cBasePlayer
    '    Public ONE_SECOND_TIMER As System.Timers.Timer

    '    Public ReadOnly Property SplashBitmap() As Bitmap
    '        Get
    '            Dim str As Stream = Me.GetType.Module.Assembly.GetManifestResourceStream("SMT.Applications.Avacuno.Avacuno_Splash_071101.png")
    '            Return New Bitmap(str)
    '        End Get
    '    End Property

    '#End Region 'PROPERTIES

    '#Region "FUNCTIONALITY"

    '#Region "FUNCTIONALITY:PLAYER"

  

    '#Region "FUNCTIONALITY:DONGLE"

    '    Private Dongle As cAvacunoDongle
    '    Public CurrentPermissions As sAvacunoFeaturePermissions
    '    Public LicensedPhoenixMode As cAvacunoDongle.eAvacunoDongleFeatures
    '    Public NonDongledMode As Boolean = False

    '    Public Structure sAvacunoFeaturePermissions
    '        Public RunFull As Boolean
    '        Public QueueOnly As Boolean
    '    End Structure

    '    Private Sub InitializeDongle()
    '        Try
    '            If AppExpiredInReg() Then
    '                Process.GetCurrentProcess.Kill() 'zero tolerance, zero notification. extreme prejudice.
    '            End If

    '            Dongle = New cAvacunoDongle
    '            If Not Dongle.GetLicense() = cSafeNetDongle.eSafeNetStatusCodes.SP_SUCCESS Then
    '                If Not IsNonDongledOk() Then
    '                    DongleFailure()
    '                Else
    '                    NonDongledMode = True
    '                    Me.Title &= "   * NON DONGLED MODE *"
    '                End If
    '            End If
    '            InitializeFeaturePermissions()
    '            DongleCheck()
    '        Catch ex As Exception
    '            Me.DongleFailure()
    '        End Try
    '    End Sub

    '    Private Sub InitializeFeaturePermissions()
    '        Try
    '            Me.CurrentPermissions = New sAvacunoFeaturePermissions

    '            Me.CurrentPermissions.RunFull = Dongle.AvacunoFeatureQuery(cAvacunoDongle.eAvacunoDongleFeatures.RunFull) = cSafeNetDongle.eSafeNetStatusCodes.SP_SUCCESS
    '            If Me.CurrentPermissions.RunFull Then
    '                Me.LicensedPhoenixMode = cAvacunoDongle.eAvacunoDongleFeatures.RunFull
    '                Exit Sub
    '            End If
    '            Me.CurrentPermissions.QueueOnly = Dongle.AvacunoFeatureQuery(cAvacunoDongle.eAvacunoDongleFeatures.QueueOnly) = cSafeNetDongle.eSafeNetStatusCodes.SP_SUCCESS
    '            If Me.CurrentPermissions.QueueOnly Then
    '                Me.LicensedPhoenixMode = cAvacunoDongle.eAvacunoDongleFeatures.QueueOnly
    '                Exit Sub
    '            End If
    '        Catch ex As Exception
    '            MessageBox.Show("Problem with InitializeFeaturePermissions(). This is a fatal error. The application will close.", My.Settings.APPLICATION_NAME, MessageBoxButton.OK, MessageBoxImage.Stop)
    '            MsgBox(ex.Message & vbNewLine & vbNewLine & ex.StackTrace)
    '            Process.GetCurrentProcess.Kill()
    '        End Try
    '    End Sub

    '    Private Sub DongleCheck()
    '        If NonDongledMode Then Exit Sub
    '        If Dongle Is Nothing Then Exit Sub
    '        If Not Dongle.AvacunoFeatureQuery(Me.LicensedPhoenixMode) = cSafeNetDongle.eSafeNetStatusCodes.SP_SUCCESS Then
    '            DongleFailure()
    '        End If
    '        If Not Me.ONE_SECOND_TIMER.Enabled Then Me.ONE_SECOND_TIMER.Start()
    '    End Sub

    '    Private Sub DongleFailure()
    '        If Dongle Is Nothing Then Exit Sub
    '        Me.ONE_SECOND_TIMER.Stop()
    '        Me.PERIODIC_DONGLE_CHECK_FAILURE_TIMEOUT = New System.Windows.Forms.Timer
    '        Me.PERIODIC_DONGLE_CHECK_FAILURE_TIMEOUT.Interval = 10000
    '        Me.PERIODIC_DONGLE_CHECK_FAILURE_TIMEOUT.Start()

    '        If MessageBox.Show("Dongle authorization failed. Retry?", My.Settings.APPLICATION_NAME, MessageBoxButton.YesNo, MessageBoxImage.Stop) = MsgBoxResult.Retry Then
    '            Me.PERIODIC_DONGLE_CHECK_FAILURE_TIMEOUT.Stop()
    '            If Not Dongle.GetLicense() = cSafeNetDongle.eSafeNetStatusCodes.SP_SUCCESS Then GoTo Failure
    '            DongleCheck()
    '        Else
    'Failure:
    '            Process.GetCurrentProcess.Kill()
    '        End If
    '    End Sub

    '    Private Function IsNonDongledOk() As Boolean
    '        'check to see if this machine is enabled to run in NonDongledMode
    '        Try

    'Check0:
    '            'CHECK HD SERIAL AGAINST TABLE
    '            Dim Ser As UInteger = GetVolumeSerial("c")
    '            If Not eAuthorizedMachines.IsDefined(GetType(eAuthorizedMachines), Ser) Then
    '                GoTo Check1
    '            Else
    '                'check the date
    '                Dim nm As String = [Enum].GetName(GetType(eAuthorizedMachines), Ser)
    '                Dim edt As eAuthorizedMachineDates = [Enum].Parse(GetType(eAuthorizedMachineDates), nm)
    '                Dim dtticks As Long = CLng(edt)
    '                If dtticks < 10000 Then
    '                    Return True
    '                Else
    '                    If dtticks > Date.Now.Ticks Then
    '                        'the user's permission has not expired
    '                        Return True
    '                    Else
    '                        SaveTimedOutInReg()
    '                        GoTo ReturnFalse
    '                    End If
    '                End If
    '            End If

    'Check1:
    '            'CHECK REG FOR DEVELOPER REG ENTRY
    '            If CheckForDeveloperRegEntry() Then
    '                Return True
    '            End If

    'Check2:
    '            'we are now going to enforce dongle protection not through shutting the app
    '            'but burning in a nag. 
    '            Return True 'let avacuno run, keystone will burn in nag

    'ReturnFalse:
    '            Return False
    '        Catch ex As Exception
    '            Debug.WriteLine("Problem with IsNonDongledOk(). Error: " & ex.Message)
    '            Return False
    '        End Try
    '    End Function

    '    Private WithEvents PERIODIC_DONGLE_CHECK_FAILURE_TIMEOUT As System.Windows.Forms.Timer
    '    Private Sub HandleDongleFailureTimeout(ByVal sender As Object, ByVal e As EventArgs) Handles PERIODIC_DONGLE_CHECK_FAILURE_TIMEOUT.Tick
    '        Process.GetCurrentProcess.Kill()
    '    End Sub

    '    Private Enum eAuthorizedMachines As UInteger 'must be dec, not hex for some reason
    '        ZOO_1 = 813752041
    '        'TRPF_Gecko_XPS = 4165273721
    '    End Enum

    '    Private Enum eAuthorizedMachineDates As Long
    '        'values below 10000 have no timeout date
    '        ZOO_1 = 633162528000000000
    '        TRPF_Gecko_XPS = 0
    '    End Enum

    '    Private Sub SaveTimedOutInReg()
    '        Try
    '            SetHKCRKey("CLSID\{898F5D5A-6324-4f21-8ABF-1C1C05AB5ED8}", "alsje", 1)
    '        Catch ex As Exception
    '            Debug.WriteLine("Problem with SaveDateInReg(). Error: " & ex.Message)
    '        End Try
    '    End Sub

    '    Private Function CheckForDeveloperRegEntry() As Boolean
    '        Try
    '            Dim o As Object = GetHKCRKey("CLSID\{13765083-D690-4121-ACE8-10B7FF6C4F4F}", "bkmgds") 'BMD key
    '            If Not o Is Nothing Then Return True
    '            o = GetHKCRKey("CLSID\{05E55EE3-AC37-41a5-B9C2-8AFFAA7C3D51}", "danku")
    '            If Not o Is Nothing Then Return True
    '            Return False
    '        Catch ex As Exception
    '            Debug.WriteLine("Problem with CheckForDeveloperRegEntry(). Error: " & ex.Message)
    '        End Try
    '    End Function

    '    Private Sub SetDeveloperRegEntry()
    '        Try
    '            SetHKCRKey("CLSID\{05E55EE3-AC37-41a5-B9C2-8AFFAA7C3D51}", "danku", "prego")
    '        Catch ex As Exception
    '            Debug.WriteLine("Problem with SetDeveloperRegEntry(). Error: " & ex.Message)
    '        End Try
    '    End Sub

    '    Private Function AppExpiredInReg() As Boolean
    '        Try
    '            Dim Expired As Long = GetHKCRKey("CLSID\{898F5D5A-6324-4f21-8ABF-1C1C05AB5ED8}", "alsje")
    '            If Expired > 0 Then
    '                Return True
    '            Else
    '                Return False
    '            End If
    '        Catch ex As Exception
    '            Debug.WriteLine("Problem with AppExpiredInReg(). Error: " & ex.Message)
    '        End Try
    '    End Function

    '#End Region 'FUNCTIONALITY:DONGLE

    '#End Region 'FUNCTIONALITY

#End Region 'OLD AVACNO

End Class
