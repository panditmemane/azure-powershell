#
# Module manifest for module 'Az'
#
# Generated by: Microsoft Corporation
#
# Generated on: 12/11/2019
#

@{

# Script module or binary module file associated with this manifest.
# RootModule = ''

# Version number of this module.
ModuleVersion = '3.2.0'

# Supported PSEditions
CompatiblePSEditions = 'Core', 'Desktop'

# ID used to uniquely identify this module
GUID = 'd48d710e-85cb-46a1-990f-22dae76f6b5f'

# Author of this module
Author = 'Microsoft Corporation'

# Company or vendor of this module
CompanyName = 'Microsoft Corporation'

# Copyright statement for this module
Copyright = 'Microsoft Corporation. All rights reserved.'

# Description of the functionality provided by this module
Description = 'Microsoft Azure PowerShell - Cmdlets to manage resources in Azure. This module is compatible with WindowsPowerShell and PowerShell Core.

For more information about the Az module, please visit the following: https://docs.microsoft.com/en-us/powershell/azure/'

# Minimum version of the PowerShell engine required by this module
PowerShellVersion = '5.1'

# Name of the PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
DotNetFrameworkVersion = '4.7.2'

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# CLRVersion = ''

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
RequiredModules = @(@{ModuleName = 'Az.Accounts'; ModuleVersion = '1.6.5'; }, 
               @{ModuleName = 'Az.Advisor'; RequiredVersion = '1.1.1'; }, 
               @{ModuleName = 'Az.Aks'; RequiredVersion = '1.0.3'; }, 
               @{ModuleName = 'Az.AnalysisServices'; RequiredVersion = '1.1.2'; }, 
               @{ModuleName = 'Az.ApiManagement'; RequiredVersion = '1.3.3'; }, 
               @{ModuleName = 'Az.ApplicationInsights'; RequiredVersion = '1.0.3'; }, 
               @{ModuleName = 'Az.Automation'; RequiredVersion = '1.3.5'; }, 
               @{ModuleName = 'Az.Batch'; RequiredVersion = '2.0.2'; }, 
               @{ModuleName = 'Az.Billing'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.Cdn'; RequiredVersion = '1.4.1'; }, 
               @{ModuleName = 'Az.CognitiveServices'; RequiredVersion = '1.2.2'; }, 
               @{ModuleName = 'Az.Compute'; RequiredVersion = '3.2.0'; }, 
               @{ModuleName = 'Az.ContainerInstance'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.ContainerRegistry'; RequiredVersion = '1.1.1'; }, 
               @{ModuleName = 'Az.DataBoxEdge'; RequiredVersion = '1.0.1'; }, 
               @{ModuleName = 'Az.DataFactory'; RequiredVersion = '1.5.1'; }, 
               @{ModuleName = 'Az.DataLakeAnalytics'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.DataLakeStore'; RequiredVersion = '1.2.6'; }, 
               @{ModuleName = 'Az.DeploymentManager'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.DevTestLabs'; RequiredVersion = '1.0.1'; }, 
               @{ModuleName = 'Az.Dns'; RequiredVersion = '1.1.2'; }, 
               @{ModuleName = 'Az.EventGrid'; RequiredVersion = '1.2.3'; }, 
               @{ModuleName = 'Az.EventHub'; RequiredVersion = '1.4.2'; }, 
               @{ModuleName = 'Az.FrontDoor'; RequiredVersion = '1.3.0'; }, 
               @{ModuleName = 'Az.HDInsight'; RequiredVersion = '3.0.1'; }, 
               @{ModuleName = 'Az.HealthcareApis'; RequiredVersion = '1.0.1'; }, 
               @{ModuleName = 'Az.IotHub'; RequiredVersion = '2.0.1'; }, 
               @{ModuleName = 'Az.KeyVault'; RequiredVersion = '1.4.0'; }, 
               @{ModuleName = 'Az.LogicApp'; RequiredVersion = '1.3.2'; }, 
               @{ModuleName = 'Az.MachineLearning'; RequiredVersion = '1.1.2'; }, 
               @{ModuleName = 'Az.ManagedServices'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.MarketplaceOrdering'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.Media'; RequiredVersion = '1.1.1'; }, 
               @{ModuleName = 'Az.Monitor'; RequiredVersion = '1.5.0'; }, 
               @{ModuleName = 'Az.Network'; RequiredVersion = '2.2.0'; }, 
               @{ModuleName = 'Az.NotificationHubs'; RequiredVersion = '1.1.1'; }, 
               @{ModuleName = 'Az.OperationalInsights'; RequiredVersion = '1.3.4'; }, 
               @{ModuleName = 'Az.PolicyInsights'; RequiredVersion = '1.1.4'; }, 
               @{ModuleName = 'Az.PowerBIEmbedded'; RequiredVersion = '1.1.1'; }, 
               @{ModuleName = 'Az.PrivateDns'; RequiredVersion = '1.0.2'; }, 
               @{ModuleName = 'Az.RecoveryServices'; RequiredVersion = '2.2.0'; }, 
               @{ModuleName = 'Az.RedisCache'; RequiredVersion = '1.2.1'; }, 
               @{ModuleName = 'Az.Relay'; RequiredVersion = '1.0.3'; }, 
               @{ModuleName = 'Az.Resources'; RequiredVersion = '1.9.0'; }, 
               @{ModuleName = 'Az.ServiceBus'; RequiredVersion = '1.4.1'; }, 
               @{ModuleName = 'Az.ServiceFabric'; RequiredVersion = '2.0.1'; }, 
               @{ModuleName = 'Az.SignalR'; RequiredVersion = '1.1.1'; }, 
               @{ModuleName = 'Az.Sql'; RequiredVersion = '2.1.1'; }, 
               @{ModuleName = 'Az.SqlVirtualMachine'; RequiredVersion = '1.0.1'; }, 
               @{ModuleName = 'Az.Storage'; RequiredVersion = '1.10.0'; }, 
               @{ModuleName = 'Az.StorageSync'; RequiredVersion = '1.2.2'; }, 
               @{ModuleName = 'Az.StreamAnalytics'; RequiredVersion = '1.0.1'; }, 
               @{ModuleName = 'Az.TrafficManager'; RequiredVersion = '1.0.3'; }, 
               @{ModuleName = 'Az.Websites'; RequiredVersion = '1.5.1'; })

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
FunctionsToExport = @()

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = @()

# Variables to export from this module
# VariablesToExport = @()

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = @()

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = 'Azure','ARM','ResourceManager','Linux','AzureAutomationNotSupported'

        # A URL to the license for this module.
        LicenseUri = 'https://aka.ms/azps-license'

        # A URL to the main website for this project.
        ProjectUri = 'https://github.com/Azure/azure-powershell'

        # A URL to an icon representing this module.
        # IconUri = ''

        # ReleaseNotes of this module
        ReleaseNotes = '3.2.0 - December 2019
Az.Accounts
* Update references in .psd1 to use relative path
* Set correct UserAgent for client-side telemetry for Az 4.0 preview
* Display user friendly error message when context is null in Az 4.0

Az.Advisor
* Update references in .psd1 to use relative path

Az.Aks
* Update references in .psd1 to use relative path

Az.AnalysisServices
* Update references in .psd1 to use relative path

Az.ApiManagement
* Update references in .psd1 to use relative path

Az.ApplicationInsights
* Update references in .psd1 to use relative path

Az.Automation
* Update references in .psd1 to use relative path

Az.Batch
* Update references in .psd1 to use relative path
* Fix issue #10602, where **New-AzBatchPool** did not properly send ''VirtualMachineConfiguration.ContainerConfiguration'' or ''VirtualMachineConfiguration.DataDisks'' to the server.

Az.Billing
* Update references in .psd1 to use relative path

Az.Cdn
* Update references in .psd1 to use relative path

Az.CognitiveServices
* Update references in .psd1 to use relative path

Az.Compute
* Update references in .psd1 to use relative path

Az.ContainerInstance
* Update references in .psd1 to use relative path

Az.ContainerRegistry
* Update references in .psd1 to use relative path

Az.DataBoxEdge
* Update references in .psd1 to use relative path

Az.DataFactory
* Update ADF .Net SDK version to 4.5.0
* Update references in .psd1 to use relative path

Az.DataLakeAnalytics
* Update references in .psd1 to use relative path

Az.DataLakeStore
* Update references in .psd1 to use relative path

Az.DeploymentManager
* Update references in .psd1 to use relative path

Az.DevTestLabs
* Update references in .psd1 to use relative path

Az.Dns
* Update references in .psd1 to use relative path

Az.EventGrid
* Update references in .psd1 to use relative path

Az.EventHub
* Update references in .psd1 to use relative path

Az.FrontDoor
* Update references in .psd1 to use relative path
* Added WAF managed rules exclusion support
* Add SocketAddr to auto-complete

Az.HDInsight
* Update references in .psd1 to use relative path

Az.HealthcareApis
* Update references in .psd1 to use relative path
* Exception Handling

Az.IotHub
* Update references in .psd1 to use relative path

Az.KeyVault
* Update references in .psd1 to use relative path
* Fixed error accessing value that is potentially not set
* Elliptic Curve Cryptography Certificate Managment
    - Added support to specify the Curve for Certificate Policies

Az.LogicApp
* Update references in .psd1 to use relative path

Az.MachineLearning
* Update references in .psd1 to use relative path

Az.ManagedServices
* Update references in .psd1 to use relative path

Az.MarketplaceOrdering
* Update references in .psd1 to use relative path

Az.Media
* Update references in .psd1 to use relative path

Az.Monitor
* Update references in .psd1 to use relative path
* Adding optional argument to the Add Diagnostic Settings command. A switch argument that if present indicates that the export to Log Analytics must be to a fixed schema (a.k.a. dedicated, data type)

Az.Network
* Update references in .psd1 to use relative path
* Support for IpGroups in AzureFirewall Application,Nat & Network Rules.

Az.NotificationHubs
* Update references in .psd1 to use relative path

Az.OperationalInsights
* Update references in .psd1 to use relative path

Az.PolicyInsights
* Update references in .psd1 to use relative path

Az.PowerBIEmbedded
* Update references in .psd1 to use relative path

Az.PrivateDns
* Update references in .psd1 to use relative path

Az.RecoveryServices
* Update references in .psd1 to use relative path
* Added SoftDelete feature for VM and added tests for softdelete
* Azure Site Recovery support for Azure Disk Encryption One Pass for Azure to Azure.

Az.RedisCache
* Update references in .psd1 to use relative path

Az.Relay
* Update references in .psd1 to use relative path

Az.Resources
* Update references in .psd1 to use relative path
* Fix an issue where template deployment fails to read a template parameter if its name conflicts with some built-in parameter name.
* Updated policy cmdlets to use new api version 2019-09-01 that introduces grouping support within policy set definitions.

Az.ServiceBus
* Update references in .psd1 to use relative path

Az.ServiceFabric
* Update references in .psd1 to use relative path

Az.SignalR
* Update references in .psd1 to use relative path

Az.Sql
* Update references in .psd1 to use relative path
* Upgraded storage creation in Vulnerability Assessment auto enablement to StorageV2

Az.SqlVirtualMachine
* Update references in .psd1 to use relative path

Az.Storage
* Update references in .psd1 to use relative path
* Support generate Blob/Constainer Idenity based SAS token with Storage Context based on Oauth authentication
    - New-AzStorageContainerSASToken
    - New-AzStorageBlobSASToken
* Support revoke Storage Account User Delegation Keys, so all Idenity SAS tokens are revoked
    - Revoke-AzStorageAccountUserDelegationKeys
* Upgrade to Microsoft.Azure.Management.Storage 14.2.0, to support new API version 2019-06-01.
* Support Share QuotaGiB more than 5120 in Management plane File Share cmdlets, and add parameter alias ''Quota'' to parameter ''QuotaGiB'' 
	- New-AzRmStorageShare
	- Update-AzRmStorageShare
* Add parameter alias ''QuotaGiB'' to parameter ''Quota''
	- Set-AzStorageShareQuota
* Fix the issue that Set-AzStorageContainerAcl can clean up the stored Access Policy
	- Set-AzStorageContainerAcl

Az.StorageSync
* Update references in .psd1 to use relative path

Az.StreamAnalytics
* Update references in .psd1 to use relative path

Az.TrafficManager
* Update references in .psd1 to use relative path

Az.Websites
* Update references in .psd1 to use relative path
'

        # Prerelease string of this module
        # Prerelease = ''

        # Flag to indicate whether the module requires explicit user acceptance for install/update/save
        # RequireLicenseAcceptance = $false

        # External dependent modules of this module
        # ExternalModuleDependencies = @()

    } # End of PSData hashtable

 } # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

