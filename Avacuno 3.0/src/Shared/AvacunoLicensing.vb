Imports Microsoft.Licensing
Imports System.Windows.Forms

'This class provides some basic licensing information and services.
Public Class cAvacunoLicensing

    Private Const _permutationID As String = "591f2"
    Private Const _productName As String = "Skunkworks"
    Private Const _productVersion As String = "1"

    Private _SLM As SLMRuntime = Nothing

    Public ReadOnly Property SkunkworksLicense() As eSkunkworksLicense
        Get
            If GetValidLicense() Is Nothing Then Return eSkunkworksLicense.UNLICENSED
            If Not IsValid Then Return eSkunkworksLicense.UNLICENSED
            Select Case PackageName.ToLower
                Case "stream"
                    Return eSkunkworksLicense.BD
                Case "dvd"
                    Return eSkunkworksLicense.DVD
                Case Else
                    Return eSkunkworksLicense.UNLICENSED
            End Select
        End Get
    End Property

    'This property represent a single entry point for access into the SLP runtime API 
    Public ReadOnly Property SLM() As SLMRuntime
        Get
            If _SLM Is Nothing Then
                _SLM = New SLMRuntime(_permutationID)
                _SLM.OpenSession(_productName, _productVersion)
            End If
            Return _SLM
        End Get
    End Property

    'This method returns a valid license for the Medical Image System demo. 
    'The license returned should not be cached because it can be affected by user actions such as deletion, etc. 
    'If multiple valid licenses exist, returns the first license 
    Public Function GetValidLicense() As ILicense
        Try
            'get a list of licenses for this product issued by the SLM specified permutation ID 
            Dim Licenses As ILicenseCollection = SLM.Licenses.GetLicenses(_productName, _productVersion, True)

            'get the first valid license 
            For Each license As ILicense In Licenses
                If license.State = LicenseState.Valid Then
                    If license.ProductName.ToLower = "skunkworks" Then
                        Return license
                    End If
                End If
            Next
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try

        Return Nothing
    End Function

    'This property returns the number of valid licenses on the machine 
    Public ReadOnly Property ValidLicenseCount() As Integer
        Get
            Dim count As Integer = 0

            Try
                'get a list of licenses for this product issue by the SLM specified permutation ID 
                Dim Licenses As ILicenseCollection = SLM.Licenses.GetLicenses(_productName, _productVersion, True)

                'count the number of valid licenses 
                For Each license As ILicense In Licenses
                    If license.State = LicenseState.Valid Then
                        count += 1
                    End If
                Next
            Catch e As Exception
                MessageBox.Show(e.Message)
            End Try

            Return count
        End Get
    End Property

    'This property returns a string representing the number of days left in the license 
    Public ReadOnly Property TimeLeft() As String
        Get
            Dim License As ILicense = GetValidLicense()
            If License IsNot Nothing Then
                Dim ExpireDate As DateTime = License.Limitations.ExpireDate
                Dim CurrentDate As DateTime = DateTime.Now

                'check if the expiration date is unlimited 
                If License.Limitations.IsUnlimited(ExpireDate) Then
                    Return "Unlimited"
                ElseIf ExpireDate > CurrentDate Then
                    'return the number of days left 
                    Dim DaysLeft As TimeSpan = ExpireDate - CurrentDate
                    Return DaysLeft.Days.ToString() + " days"
                Else
                    Return "0 days"
                End If
            Else
                Return Nothing
            End If
        End Get
    End Property

    'This method displays the license activation wizard, which allows users to renew their current license online 
    Public Sub RenewLicense(ByVal parentWindow As IWin32Window)
        Try
            'create an instance of the activation wizard 
            Using Dlg As IActivationWizard = SLM.Activation.CreateActivationDialog(_productName, _productVersion)

                Dim License As ILicense = GetValidLicense()
                If License IsNot Nothing Then
                    'select online activation as the operation to be performed 
                    Dlg.SelectedOperation = ActivationWizardOperation.OnlineActivation

                    'set the activation license to the current license 
                    Dlg.LicenseKey = License.ActivationKey

                    'start with the renewal confirmation page of the activation wizard 
                    Dlg.NavigateToPage(ActivationWizardPage.LicenseKeyEntry, True)

                    'display the activation wizard 
                    Dlg.ShowDialog(parentWindow)
                End If
            End Using
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

    Public Sub GetLicenseRequestData(ByVal parentWindow As IWin32Window)
        Try
            'create an instance of the activation wizard 
            Using Dlg As IActivationWizard = SLM.Activation.CreateActivationDialog(_productName, _productVersion)

                Dim License As ILicense = GetValidLicense()
                If License IsNot Nothing Then
                    'select online activation as the operation to be performed 
                    Dlg.SelectedOperation = ActivationWizardOperation.RequestLicenseFile

                    'set the activation license to the current license 
                    Dlg.LicenseKey = License.ActivationKey

                    'start with the renewal confirmation page of the activation wizard 
                    Dlg.NavigateToPage(ActivationWizardPage.LicenseRequestCreation, True)

                    'display the activation wizard 
                    Dlg.ShowDialog(parentWindow)
                End If
            End Using
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

    Public Sub SelectLicenseRequestFile(ByVal parentWindow As IWin32Window)
        Try
            'create an instance of the activation wizard 
            Using Dlg As IActivationWizard = SLM.Activation.CreateActivationDialog(_productName, _productVersion)

                Dim License As ILicense = GetValidLicense()
                If License IsNot Nothing Then
                    'select online activation as the operation to be performed 
                    Dlg.SelectedOperation = ActivationWizardOperation.InstallLicenseFile

                    'set the activation license to the current license 
                    Dlg.LicenseKey = License.ActivationKey

                    'start with the renewal confirmation page of the activation wizard 
                    Dlg.NavigateToPage(ActivationWizardPage.LicenseFileEntry, True)

                    'display the activation wizard 
                    Dlg.ShowDialog(parentWindow)
                End If
            End Using
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

    Public Sub ActivationDialog(ByVal parentWindow As IWin32Window)
        Try
            'create an instance of the activation wizard 
            Using Dlg As IActivationWizard = SLM.Activation.CreateActivationDialog(_productName, _productVersion)

                'select online activation as the operation to be performed 
                Dlg.SelectedOperation = ActivationWizardOperation.OnlineActivation

                'start with the renewal confirmation page of the activation wizard 
                Dlg.NavigateToPage(ActivationWizardPage.LicenseKeyEntry, True)

                'display the activation wizard 
                Dlg.ShowDialog(parentWindow)
            End Using
        Catch e As Exception
            MessageBox.Show(e.Message)
        End Try
    End Sub

    Public ReadOnly Property StartDate() As Date
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.StartDate
            'If License IsNot Nothing Then
            '    Dim exDate As DateTime = License.Limitations.ExpireDate
            '    'check if the expiration date is unlimited 
            '    If License.Limitations.IsUnlimited(exDate) Then
            '        Return Nothing
            '    Else
            '        Return exDate
            '    End If
            'Else
            '    Return Nothing
            'End If
        End Get
    End Property

    Public ReadOnly Property ExpireDate() As Date
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.ActualExpirationDate
            'If License IsNot Nothing Then
            '    Dim exDate As DateTime = License.Limitations.ExpireDate
            '    'check if the expiration date is unlimited 
            '    If License.Limitations.IsUnlimited(exDate) Then
            '        Return Nothing
            '    Else
            '        Return exDate
            '    End If
            'Else
            '    Return Nothing
            'End If
        End Get
    End Property

    Public ReadOnly Property Features() As List(Of String)
        Get
            Dim License As ILicense = GetValidLicense()
            Dim out As New List(Of String)
            For Each Feature As IFeature In License.Features
                out.Add(Feature.Name)
            Next
            Return out
        End Get
    End Property

    Public ReadOnly Property Key() As String
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.ActivationKey
        End Get
    End Property

    Public ReadOnly Property PackageName() As String
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.PackageName
        End Get
    End Property

    Public ReadOnly Property LicenseLevel() As String
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.LicenseLevel.ToString()
        End Get
    End Property

    Public ReadOnly Property ProductName() As String
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.ProductName
        End Get
    End Property

    Public ReadOnly Property ProductVersion() As String
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.ProductVersion
        End Get
    End Property

    Public ReadOnly Property IsTrial() As Boolean
        Get
            Return LicenseLevel.ToLower <> "commercial"
        End Get
    End Property

    Public ReadOnly Property IsValid() As Boolean
        Get
            Dim License As ILicense = GetValidLicense()
            Return License.IsValid
        End Get
    End Property

End Class
