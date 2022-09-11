$currentDir = (pwd).path
$detected = $false
if($Args.Length -ge 1) {
    $currentDir = $Args[0].trim("'")
}
if($Args.Length -ge 2) {
    $targetDir = $Args[1].trim("'")
    $detected = $true
}
if($Args.Length -ge 3) {
    $exeFile = $Args[2].trim("'")
}
Write-Output $currentDir
Write-Output $targetDir
Write-Output $exeFile
If (-not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')){
    Start-Process -FilePath PowerShell.exe -ArgumentList "-WindowStyle Hidden -NoLogo -ExecutionPolicy Bypass -File $($MyInvocation.MyCommand.Path) ""'$currentDir'"" ""'$targetDir'"" ""'$exeFile'""" -Verb RunAs
    Exit
}

[void][System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")

if($detected -eq $false) {
    Add-Type -AssemblyName System.Windows.Forms
    Add-Type -AssemblyName System.Drawing

    $form = New-Object System.Windows.Forms.Form
    $form.Text = 'インストール先を選択'
    $form.Size = New-Object System.Drawing.Size(300,200)
    $form.StartPosition = 'CenterScreen'

    $okButton = New-Object System.Windows.Forms.Button
    $okButton.Location = New-Object System.Drawing.Point(75,120)
    $okButton.Size = New-Object System.Drawing.Size(75,23)
    $okButton.Text = 'OK'
    $okButton.DialogResult = [System.Windows.Forms.DialogResult]::OK
    $form.AcceptButton = $okButton
    $form.Controls.Add($okButton)

    $cancelButton = New-Object System.Windows.Forms.Button
    $cancelButton.Location = New-Object System.Drawing.Point(150,120)
    $cancelButton.Size = New-Object System.Drawing.Size(75,23)
    $cancelButton.Text = 'キャンセル'
    $cancelButton.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
    $form.CancelButton = $cancelButton
    $form.Controls.Add($cancelButton)

    $label = New-Object System.Windows.Forms.Label
    $label.Location = New-Object System.Drawing.Point(10,20)
    $label.Size = New-Object System.Drawing.Size(280,20)
    $label.Text = 'インストール先を選択'
    $form.Controls.Add($label)

    $listBox = New-Object System.Windows.Forms.ListBox
    $listBox.Location = New-Object System.Drawing.Point(10,40)
    $listBox.Size = New-Object System.Drawing.Size(260,20)
    $listBox.Height = 80

    if((Test-Path "$currentDir\Input Devices\Kusaanko.NumerousControllerInterface.NET4.dll") -eq "True"){
        # Bve6
        if((Test-Path "C:\Program Files\mackoy\BveTs6\BveTs.exe") -eq "True"){
            [void] $listBox.Items.Add("C:\Program Files\mackoy\BveTs6\")
            $detected = $true
        }
    }else {
        # Bve5
        if((Test-Path "C:\Program Files (x86)\mackoy\BveTs5\BveTs.exe") -eq "True"){
            [void] $listBox.Items.Add("C:\Program Files (x86)\mackoy\BveTs5\")
            $detected = $true
        }
    }
    [void] $listBox.Items.Add("その他")

    $form.Controls.Add($listBox)

    $form.Topmost = $true

    $result = $form.ShowDialog()

    if ($result -eq [System.Windows.Forms.DialogResult]::OK)
    {
        $x = $listBox.SelectedItem
        if($x -eq "その他") {
            $detected = $false
        }else {
            $targetDir = $x
        }
    }else {
        Exit
    }
}

if($detected -eq $false) {
    $dialog = New-Object System.Windows.Forms.OpenFileDialog
    $dialog.Filter = "Bve 実行ファイル(*.exe)|*.exe"
    $dialog.InitialDirectory = "C:\"
    $dialog.Title = "ファイルを選択してください"
    
    if($dialog.ShowDialog() -eq [System.Windows.Forms.DialogResult]::OK){
        $targetDir = Split-Path $dialog.FileName -Parent
    }
}

try {
    $plugin = $false
    try {
        Copy-Item -Path "$currentDir\Input Devices\Kusaanko.NumerousControllerInterface.dll" -Destination ($targetDir + "\Input Devices") -Force -ErrorAction Stop
        $plugin = $true
    }catch {}
    try {
        Copy-Item -Path "$currentDir\Input Devices\Kusaanko.NumerousControllerInterface.NET4.dll" -Destination ($targetDir + "\Input Devices") -Force -ErrorAction Stop
        $plugin = $true
    }catch {}
    if ($plugin -eq $false) {
        throw "Plugin is not exist"
    }
    Copy-Item -Path "$currentDir\LibUsbDotNet.dll" -Destination $targetDir -Force -ErrorAction Stop
    Copy-Item -Path "$currentDir\Newtonsoft.Json.dll" -Destination $targetDir -Force -ErrorAction Stop
    [System.Windows.Forms.MessageBox]::Show("インストールが完了しました", "NumerousControllerInterface")
}catch {
    [System.Windows.Forms.MessageBox]::Show("必要なファイルが無かったかインストール先が存在しないため、インストールに失敗しました", "NumerousControllerInterface")
}

if($Args.Length -ge 3) {
    Start-Process -FilePath "$exeFile" -WindowStyle Normal
    Exit
}
