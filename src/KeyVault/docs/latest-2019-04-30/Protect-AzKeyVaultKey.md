---
external help file:
Module Name: Az.KeyVault
online version: https://docs.microsoft.com/en-us/powershell/module/az.keyvault/protect-azkeyvaultkey
schema: 2.0.0
---

# Protect-AzKeyVaultKey

## SYNOPSIS
The ENCRYPT operation encrypts an arbitrary sequence of bytes using an encryption key that is stored in Azure Key Vault.
Note that the ENCRYPT operation only supports a single block of data, the size of which is dependent on the target key and the encryption algorithm to be used.
The ENCRYPT operation is only strictly necessary for symmetric keys stored in Azure Key Vault since protection with an asymmetric key can be performed using public portion of the key.
This operation is supported for asymmetric keys as a convenience for callers that have a key-reference but do not have access to the public key material.
This operation requires the keys/encrypt permission.

## SYNTAX

### Encrypt (Default)
```
Protect-AzKeyVaultKey -KeyName <String> -KeyVersion <String> [-KeyVaultDnsSuffix <String>]
 [-VaultName <String>] [-Parameter <IKeyOperationsParameters>] [-DefaultProfile <PSObject>] [-Confirm]
 [-WhatIf] [<CommonParameters>]
```

### EncryptExpanded
```
Protect-AzKeyVaultKey -KeyName <String> -KeyVersion <String> -Algorithm <JsonWebKeyEncryptionAlgorithm>
 -InputFile <String> [-KeyVaultDnsSuffix <String>] [-VaultName <String>] [-DefaultProfile <PSObject>]
 [-Confirm] [-WhatIf] [<CommonParameters>]
```

### EncryptViaIdentityExpanded
```
Protect-AzKeyVaultKey -InputObject <IKeyVaultIdentity> -Algorithm <JsonWebKeyEncryptionAlgorithm>
 -InputFile <String> [-KeyVaultDnsSuffix <String>] [-VaultName <String>] [-DefaultProfile <PSObject>]
 [-Confirm] [-WhatIf] [<CommonParameters>]
```

### EncryptViaIdentity
```
Protect-AzKeyVaultKey -InputObject <IKeyVaultIdentity> [-KeyVaultDnsSuffix <String>] [-VaultName <String>]
 [-Parameter <IKeyOperationsParameters>] [-DefaultProfile <PSObject>] [-Confirm] [-WhatIf]
 [<CommonParameters>]
```

## DESCRIPTION
The ENCRYPT operation encrypts an arbitrary sequence of bytes using an encryption key that is stored in Azure Key Vault.
Note that the ENCRYPT operation only supports a single block of data, the size of which is dependent on the target key and the encryption algorithm to be used.
The ENCRYPT operation is only strictly necessary for symmetric keys stored in Azure Key Vault since protection with an asymmetric key can be performed using public portion of the key.
This operation is supported for asymmetric keys as a convenience for callers that have a key-reference but do not have access to the public key material.
This operation requires the keys/encrypt permission.

## EXAMPLES

### Example 1: {{ Add title here }}
```powershell
PS C:\> {{ Add code here }}

{{ Add output here }}
```

{{ Add description here }}

### Example 2: {{ Add title here }}
```powershell
PS C:\> {{ Add code here }}

{{ Add output here }}
```

{{ Add description here }}

## PARAMETERS

### -Algorithm
algorithm identifier

```yaml
Type: Microsoft.Azure.PowerShell.Cmdlets.KeyVault.Support.JsonWebKeyEncryptionAlgorithm
Parameter Sets: EncryptExpanded, EncryptViaIdentityExpanded
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -DefaultProfile
The credentials, account, tenant, and subscription used for communication with Azure.

```yaml
Type: System.Management.Automation.PSObject
Parameter Sets: (All)
Aliases: AzureRMContext, AzureCredential

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -InputFile
Input File for Value (HELP MESSAGE MISSING)

```yaml
Type: System.String
Parameter Sets: EncryptExpanded, EncryptViaIdentityExpanded
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -InputObject
Identity Parameter

```yaml
Type: Microsoft.Azure.PowerShell.Cmdlets.KeyVault.Models.IKeyVaultIdentity
Parameter Sets: EncryptViaIdentityExpanded, EncryptViaIdentity
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
Dynamic: False
```

### -KeyName
The name of the key.

```yaml
Type: System.String
Parameter Sets: Encrypt, EncryptExpanded
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -KeyVaultDnsSuffix
MISSING DESCRIPTION 06

```yaml
Type: System.String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -KeyVersion
The version of the key.

```yaml
Type: System.String
Parameter Sets: Encrypt, EncryptExpanded
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -Parameter
The key operations parameters.
To construct, see NOTES section for PARAMETER properties and create a hash table.

```yaml
Type: Microsoft.Azure.PowerShell.Cmdlets.KeyVault.Models.Api20161001.IKeyOperationsParameters
Parameter Sets: Encrypt, EncryptViaIdentity
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
Dynamic: False
```

### -VaultName
MISSING DESCRIPTION 06

```yaml
Type: System.String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -Confirm
Prompts you for confirmation before running the cmdlet.

```yaml
Type: System.Management.Automation.SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### -WhatIf
Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: System.Management.Automation.SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
Dynamic: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### Microsoft.Azure.PowerShell.Cmdlets.KeyVault.Models.IKeyVaultIdentity

### Microsoft.Azure.PowerShell.Cmdlets.KeyVault.Models.Api20161001.IKeyOperationsParameters

## OUTPUTS

### Microsoft.Azure.PowerShell.Cmdlets.KeyVault.Models.Api20161001.IKeyOperationResult

## ALIASES

## NOTES

### COMPLEX PARAMETER PROPERTIES
To create the parameters described below, construct a hash table containing the appropriate properties. For information on hash tables, run Get-Help about_Hash_Tables.

#### PARAMETER <IKeyOperationsParameters>: The key operations parameters.
  - `Algorithm <JsonWebKeyEncryptionAlgorithm>`: algorithm identifier
  - `Value <Byte[]>`: 

## RELATED LINKS
