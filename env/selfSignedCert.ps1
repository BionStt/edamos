$root = New-SelfSignedCertificate `
    -Subject "EdamosRootCA" `
    -KeyAlgorithm RSA `
    -KeyLength 2048 `
    -NotBefore (Get-Date) `
    -NotAfter (Get-Date).AddYears(2) `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -HashAlgorithm SHA256 `
    -KeyUsage DigitalSignature, CertSign     

$cert = New-SelfSignedCertificate `
    -Subject "edamos.example.com" `
    -DnsName login.edamos.example.com, upload.edamos.example.com, download.edamos.example.com, api.edamos.example.com, public.edamos.example.com, admin.edamos.example.com `
    -KeyAlgorithm RSA `
    -KeyLength 2048 `
    -NotBefore (Get-Date) `
    -NotAfter (Get-Date).AddYears(2) `
    -CertStoreLocation "cert:\LocalMachine\My" `
    -HashAlgorithm SHA256 `
	-Signer $root `
    -KeyUsage KeyEncipherment, DigitalSignature 


#New-SelfSignedCertificate -DnsName "sample.dhapcis4anc2.com", "login.dhapcis4anc2.com" -CertStoreLocation "cert:\LocalMachine\My" -KeyUsage KeyEncipherment,DigitalSignature -Provider "Microsoft Software Key Storage Provider" -NotAfter $([datetime]::now.AddYears(5))

#Add it to trusted
$rootStore = New-Object System.Security.Cryptography.X509Certificates.X509Store -ArgumentList Root, LocalMachine
$rootStore.Open("MaxAllowed")
$rootStore.Add($root)
$rootStore.Close()

#Print cert details
$root
$cert

$mypwd = ConvertTo-SecureString -String "qwe123" -Force -AsPlainText

New-Item -ItemType directory -Path c:\certtemp -Force
Export-PfxCertificate -FilePath c:\certtemp\root.pfx -ChainOption EndEntityCertOnly -Password $mypwd -Cert $root
Export-Certificate -Cert $root -FilePath c:\certtemp\root.cer

certutil -encode c:\certtemp\root.cer c:\certtemp\root.crt

Export-PfxCertificate -FilePath c:\certtemp\cert.pfx -ChainOption EndEntityCertOnly -Password $mypwd -Cert $cert
Export-Certificate -Cert $cert -FilePath c:\certtemp\cert.cer