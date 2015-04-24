﻿﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Management.Automation;
using System.Net;
using Microsoft.Azure.Commands.Automation.Properties;
using Microsoft.Azure.Commands.Automation.Model;
using Microsoft.Azure.Management.Automation;
using AutomationManagement = Microsoft.Azure.Management.Automation;
using Microsoft.Azure.Management.Automation.Models;
using Newtonsoft.Json;
using Hyak.Common;
using System.Management.Automation.Runspaces;
using DscNode = Microsoft.Azure.Management.Automation.Models.DscNode;
using Job = Microsoft.Azure.Management.Automation.Models.Job;

 namespace Microsoft.Azure.Commands.Automation.Common
{
    

    using DscNode = Microsoft.Azure.Management.Automation.Models.DscNode;

    public partial class AutomationClient : IAutomationClient
    {
        #region DscConfiguration Operations

        public IEnumerable<Model.DscConfiguration> ListDscConfigurations(
            string resourceGroupName,
            string automationAccountName)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();

            return AutomationManagementClient.ContinuationTokenHandler(
                skipToken =>
                    {
                        var response = this.automationManagementClient.Configurations.List(
                            resourceGroupName,
                            automationAccountName);
                        return new ResponseWithSkipToken<AutomationManagement.Models.DscConfiguration>(
                            response,
                            response.Configurations);
                    }).Select(c => new Model.DscConfiguration(resourceGroupName, automationAccountName, c));
        }

        public Model.DscConfiguration GetConfiguration(
            string resourceGroupName,
            string automationAccountName,
            string configurationName)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("ConfigurationName", configurationName).NotNull();

            var configuration =
                this.automationManagementClient.Configurations.Get(
                    resourceGroupName,
                    automationAccountName,
                    configurationName).Configuration;

            return new Model.DscConfiguration(resourceGroupName, automationAccountName, configuration);
        }

        public Model.DscConfiguration CreateConfiguration(
            string resourceGroupName,
            string automationAccountName,
            string sourcePath,
            IDictionary tags, 
            string description,
            bool? logVerbose,
            bool published,
            bool overWrite)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("SourcePath", sourcePath).NotNull();

            string fileContent = null;
            string configurationName = String.Empty;

            try
            {
                if (File.Exists(Path.GetFullPath(sourcePath)))
                {
                    fileContent = System.IO.File.ReadAllText(sourcePath);
                }
            }
            catch (Exception)
            {
                // exception in accessing the file path
                throw new FileNotFoundException(
                                    string.Format(
                                        CultureInfo.CurrentCulture,
                                        Resources.ConfigurationSourcePathInvalid));
            }

            // configuration name is same as filename
            configurationName = Path.GetFileNameWithoutExtension(sourcePath);

            // for the private preivew, configuration can be imported in Published mode only
            // Draft mode is not implemented
            if (!published)
            {
                throw new NotImplementedException(
                                    string.Format(
                                        CultureInfo.CurrentCulture,
                                        Resources.ConfigurationNotPublished));
            }

            // if configuration already exists, ensure overwrite flag is specified
            var configurationModel = this.TryGetConfigurationModel(
                resourceGroupName,
                automationAccountName,
                configurationName);
            if (configurationModel != null)
            {
                if (!overWrite)
                {
                    throw new ResourceCommonException(typeof(Model.DscConfiguration),
                        string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationAlreadyExists, configurationName));
                }
            }

            // location of the configuration is set to same as that of automation account
            string location = this.GetAutomationAccount(resourceGroupName, automationAccountName).Location;

            IDictionary<string, string> configurationTags = null;
            if (tags != null) configurationTags = tags.Cast<DictionaryEntry>().ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value.ToString());

            var configurationCreateParameters = new DscConfigurationCreateOrUpdateParameters()
                                                    {
                                                        Name = configurationName,
                                                        Location = location,
                                                        Tags = configurationTags,
                                                        Properties = new DscConfigurationCreateOrUpdateProperties()
                                                                {
                                                                    Description = String.IsNullOrEmpty(description) ? String.Empty : description,
                                                                    LogVerbose = (logVerbose.HasValue) ? logVerbose.Value : false,
                                                                    Source = new Microsoft.Azure.Management.Automation.Models.ContentSource()
                                                                            {
                                                                                // only embeddedContent supported for now
                                                                                ContentType = Model.ContentSourceType.embeddedContent.ToString(),
                                                                                Value = fileContent
                                                                            }
                                                                }
                                                    };

            var configuration =
                this.automationManagementClient.Configurations.CreateOrUpdate(
                    resourceGroupName,
                    automationAccountName,
                    configurationCreateParameters).Configuration;

            return new Model.DscConfiguration(resourceGroupName, automationAccountName, configuration);
        }

        private Model.DscConfiguration TryGetConfigurationModel(string resourceGroupName, string automationAccountName, string configurationName)
        {
            Model.DscConfiguration configuration = null;
            try
            {
                configuration = this.GetConfiguration(
                                                resourceGroupName,
                                                automationAccountName,
                                                configurationName);
            }
            catch (CloudException e)
            {
                if (e.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    configuration = null;
                }
                else
                {
                    throw;
                }
            }
            return configuration;
        }

    #endregion

        #region DscMetaConfig Operations
        public DirectoryInfo GetDscMetaConfig(string resourceGroupName, string automationAccountName, string outputFolder, string[] computerNames, bool overwriteExistingFile)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();

            string outputFolderFullPath = this.GetCurrentDirectory(); // initialize with current directory;

            if (!String.IsNullOrEmpty(outputFolder))
            {
                outputFolderFullPath = this.ValidateAndGetFullPath(outputFolder);
            }

            var dscMetaConfig = this.automationManagementClient.AgentRegistrationInformation.Get(
                resourceGroupName,
                automationAccountName).AgentRegistration;

            // get the metaconfig value
            string dscMetaConfigValue = new DscOnboardingMetaconfig(resourceGroupName, automationAccountName, dscMetaConfig).DscMetaConfiguration;

            if (computerNames == null)
            {
                computerNames = new[] { "localhost" }; // No computer specified. Initialize with Localhost
            }

            string outputFilePath = String.Empty;
            const string FileExtension = ".meta.mof"; // this will be .meta.mof
            const string DscMetaConfigsFolder = "DscMetaConfigs"; // Folder name where metaconfigs are stored
            
            outputFolderFullPath = System.IO.Path.Combine(outputFolderFullPath, DscMetaConfigsFolder);

            this.CreateOutputFolder(outputFolderFullPath);
            
            foreach (string computerName in computerNames)
            {
                outputFilePath = outputFolderFullPath + "\\" + computerName + FileExtension;

                // file exists and overwrite Not specified
                if (File.Exists(outputFilePath) && !overwriteExistingFile)
                {
                    throw new ArgumentException(
                            string.Format(CultureInfo.CurrentCulture, Resources.MetaconfigAlreadyExists, outputFilePath));
                }
                
                // Write to the file
                this.WriteMetaConfigMofFile(outputFilePath, dscMetaConfigValue);
            }

            return new DirectoryInfo(outputFolderFullPath);
        }

        private void CreateOutputFolder(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    System.IO.Directory.CreateDirectory(folderPath);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException(
                        string.Format(CultureInfo.CurrentCulture, Resources.UnauthorizedAccess, folderPath));
            }
        }

        /// <summary>
        /// Get the current directory path
        /// </summary>
        /// <returns>full path of the current directory</returns>
        private string GetCurrentDirectory()
        {
            string currentDirectory = String.Empty;

            try
            {
                currentDirectory = Directory.GetCurrentDirectory();
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException(
                        string.Format(CultureInfo.CurrentCulture, Resources.UnauthorizedAccess, currentDirectory));
            }

            return currentDirectory;
        }

        /// <summary>
        /// Validate and return the full folder path
        /// </summary>
        /// <param name="folderPath">Folder path to be validated</param>
        private string ValidateAndGetFullPath(string folderPath)
        {
            string fullPath = String.Empty;

            // check if folder exists - path can be absolute or relative
            if (Directory.Exists(folderPath))
            {
                // get the full path
                fullPath  = Path.GetFullPath(folderPath);
            }
            else
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.InvalidFolderPath, folderPath));
            }
            
            return fullPath;
        }

        private void WriteMetaConfigMofFile(string outputFilePath, string fileContent)
        {
            try
            {
                File.WriteAllText(outputFilePath, fileContent);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.InvalidFilePath, outputFilePath));
            }
            catch (UnauthorizedAccessException)
            {
                throw new UnauthorizedAccessException(
                    string.Format(CultureInfo.CurrentCulture, Resources.UnauthorizedAccess, outputFilePath));
            }    
        }

        #endregion

        #region AgentRegistration Operations
        public Model.AgentRegistration GetAgentRegistration(string resourceGroupName, string automationAccountName)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
        
            var agentRegistration = this.automationManagementClient.AgentRegistrationInformation.Get(
                resourceGroupName,
                automationAccountName).AgentRegistration;

            return new Model.AgentRegistration(resourceGroupName, automationAccountName, agentRegistration);
        }

        public Model.AgentRegistration NewAgentRegistrationKey(
            string resourceGroupName,
            string automationAccountName,
            string keyType)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("KeyType", keyType).NotNull();

            AgentRegistrationRegenerateKeyParameter keyName = new AgentRegistrationRegenerateKeyParameter() { KeyName = keyType};

            var agentRegistration = this.automationManagementClient.AgentRegistrationInformation.RegenerateKey(
                resourceGroupName,
                automationAccountName,
                keyName).AgentRegistration;

            return new Model.AgentRegistration(resourceGroupName, automationAccountName, agentRegistration);
        }
        #endregion

        #region DscNode Operations
        public Model.DscNode GetDscNodeById(
            string resourceGroupName,
            string automationAccountName,
            Guid nodeId)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("NodeId", nodeId).NotNull();

            var node =
                this.automationManagementClient.Nodes.Get(
                    resourceGroupName,
                    automationAccountName,
                    nodeId).Node;

            return new Model.DscNode(resourceGroupName, automationAccountName, node);
        }

        public IEnumerable<Model.DscNode> ListDscNodesByName(
            string resourceGroupName,
            string automationAccountName,
            string nodeName,
            string status)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("NodeName", nodeName).NotNull();

            IEnumerable<AutomationManagement.Models.DscNode> dscNodes;

            if (!String.IsNullOrEmpty(status))
            {
                dscNodes = AutomationManagementClient.ContinuationTokenHandler(
                    skipToken =>
                        {
                            var response = this.automationManagementClient.Nodes.List(
                                resourceGroupName,
                                automationAccountName,
                                new DscNodeListParameters { Status = status, Name = nodeName });
                            
                            return new ResponseWithSkipToken<DscNode>(response, response.Nodes);
                        });
            }
            else
            {
                dscNodes = AutomationManagementClient.ContinuationTokenHandler(
                    skipToken =>
                    {
                        var response = this.automationManagementClient.Nodes.List(
                            resourceGroupName,
                            automationAccountName,
                            new DscNodeListParameters { Name = nodeName });

                        return new ResponseWithSkipToken<DscNode>(response, response.Nodes);
                    });
            }
            
            return dscNodes.Select(dscNode => new Model.DscNode(resourceGroupName, automationAccountName, dscNode));
        }

        public IEnumerable<Model.DscNode> ListDscNodesByNodeConfiguration(
            string resourceGroupName,
            string automationAccountName,
            string nodeConfigurationName,
            string status)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("NodeConfigurationName", nodeConfigurationName).NotNull();

            IEnumerable<AutomationManagement.Models.DscNode> dscNodes;

            if (!String.IsNullOrEmpty(status))
            {
                dscNodes = AutomationManagementClient.ContinuationTokenHandler(
                    skipToken =>
                    {
                        var response = this.automationManagementClient.Nodes.List(
                            resourceGroupName,
                            automationAccountName,
                            new DscNodeListParameters { Status = status, NodeConfigurationName = nodeConfigurationName });

                        return new ResponseWithSkipToken<DscNode>(response, response.Nodes);
                    });
            }
            else
            {
                dscNodes = AutomationManagementClient.ContinuationTokenHandler(
                    skipToken =>
                    {
                        var response = this.automationManagementClient.Nodes.List(
                            resourceGroupName,
                            automationAccountName,
                            new DscNodeListParameters { NodeConfigurationName = nodeConfigurationName });

                        return new ResponseWithSkipToken<DscNode>(response, response.Nodes);
                    });
            }

            return dscNodes.Select(dscNode => new Model.DscNode(resourceGroupName, automationAccountName, dscNode));
        }

        public IEnumerable<Model.DscNode> ListDscNodesByConfiguration(
            string resourceGroupName,
            string automationAccountName,
            string configurationName,
            string status)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("ConfigurationName", configurationName).NotNull();

            IEnumerable<Model.DscNode> listOfNodes = Enumerable.Empty<Model.DscNode>();
            
            // first get the list of node configurations for the given configuration
            IEnumerable<Model.NodeConfiguration> listOfNodeConfigurations = this.EnumerateNodeConfigurationsByConfigurationName(
                resourceGroupName,
                automationAccountName,
                configurationName);

             IEnumerable<Model.DscNode> listOfNodesForGivenNodeConfiguration;

            // for each nodeconfiguration, get the list of nodes and concatenate
            foreach (var nodeConfiguration in listOfNodeConfigurations)
            {
                listOfNodesForGivenNodeConfiguration =
                    this.ListDscNodesByNodeConfiguration(
                        resourceGroupName,
                        automationAccountName,
                        nodeConfiguration.Name, 
                        status);

                if (listOfNodesForGivenNodeConfiguration != null)
                {
                    listOfNodes = listOfNodes.Concat(listOfNodesForGivenNodeConfiguration);
                }
            }

            return listOfNodes;
        }

        public IEnumerable<Model.DscNode> ListDscNodes(
            string resourceGroupName,
            string automationAccountName,
            string status)
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();

            IEnumerable<AutomationManagement.Models.DscNode> dscNodes;

            if (!String.IsNullOrEmpty(status))
                {
                    dscNodes = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response = this.automationManagementClient.Nodes.List(
                                resourceGroupName,
                                automationAccountName,
                                new DscNodeListParameters { Status = status });

                            return new ResponseWithSkipToken<DscNode>(response, response.Nodes);
                        });
                }
                else
                {
                    dscNodes = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response = this.automationManagementClient.Nodes.List(
                                resourceGroupName,
                                automationAccountName,
                                new DscNodeListParameters {});

                            return new ResponseWithSkipToken<DscNode>(response, response.Nodes);
                        });
                }

            return dscNodes.Select(dscNode => new Model.DscNode(resourceGroupName, automationAccountName, dscNode));
        }

        public Model.DscNode SetDscNodeById(
            string resourceGroupName,
            string automationAccountName,
            Guid nodeId,
            string nodeConfigurationName
            )
        {
            Requires.Argument("ResourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("AutomationAccountName", automationAccountName).NotNull();
            Requires.Argument("NodeId", nodeId).NotNull();
            Requires.Argument("NodeConfigurationName", nodeConfigurationName).NotNull();
            
            try
            {
                var getNode = this.automationManagementClient.Nodes.Get(
                        resourceGroupName,
                        automationAccountName,
                        nodeId).Node;
            }
            catch (CloudException cloudException)
            {
                if (cloudException.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.NodeNotFound), cloudException);
                }

                throw;
            }

            // ***
            // Note: No need to check if an existing configuration is already assigned. The confirmation is obtained when the cmdlet is executed
            // *** 

            var nodeConfiguration = new DscNodeConfigurationAssociationProperty { Name = nodeConfigurationName };

            var node =
                this.automationManagementClient.Nodes.Patch(
                    resourceGroupName,
                    automationAccountName,
                    new DscNodePatchParameters
                        {
                            Id = nodeId,
                            NodeConfiguration = nodeConfiguration
                        }).Node;

            return new Model.DscNode(resourceGroupName, automationAccountName, node);            
        }

        public void DeleteDscNode(string resourceGroupName, string automationAccountName, Guid nodeId)
        {
            try
            {
                this.automationManagementClient.Nodes.Delete(
                    resourceGroupName,
                    automationAccountName,
                    nodeId);
            }
            catch (CloudException cloudException)
            {
                if (cloudException.Response.StatusCode == HttpStatusCode.NoContent)
                {
                    throw new ResourceNotFoundException(typeof(DscNode),
                        string.Format(CultureInfo.CurrentCulture, Resources.DscNodeNotFound, nodeId.ToString()));
                }

                throw;
            }
	}

        public void RegisterDscNode(string resourceGroupName,
                                            string automationAccountName,
                                            string azureVMName,
                                            string nodeconfigurationName,
                                            string configurationMode,
                                            int configurationModeFrequencyMins,
                                            int refreshFrequencyMins,
                                            bool rebootFlag,
                                            string actionAfterReboot,
                                            bool moduleOverwriteFlag,
                                            string azureVmResourceGroup,
                                            string azureVmLocation)
        {
            // get the location from AutomationAccountName. This will validate the account too
            string location = this.GetAutomationAccount(resourceGroupName, automationAccountName).Location;

            // if vm location is specified, use that
            if (!String.IsNullOrEmpty(azureVmLocation))
            {
                location = azureVmLocation;
            }

            // if azureVmResourceGroup not specified, use the resource group
            if (String.IsNullOrEmpty(azureVmResourceGroup))
            {
                azureVmResourceGroup = resourceGroupName;
            }

            string deploymentName = System.DateTimeOffset.Now.LocalDateTime.ToString("yyyyMMddhhmmss");

            // get the endpoint and keys
            Model.AgentRegistration agentRegistrationInfo = this.GetAgentRegistration(
                resourceGroupName,
                automationAccountName);

            // prepare the parameters to be used in New-AzureResourceGroupDeployment cmdlet
            Hashtable templateParameters = new Hashtable();
            templateParameters.Add("vmName", azureVMName);
            templateParameters.Add("location", location);
            templateParameters.Add("modulesUrl", Constants.ModulesUrl);
            templateParameters.Add("configurationFunction", Constants.ConfigurationFunction);
            templateParameters.Add("registrationUrl", agentRegistrationInfo.Endpoint);
            templateParameters.Add("registrationKey", agentRegistrationInfo.PrimaryKey);
            templateParameters.Add("nodeConfigurationName", nodeconfigurationName);
            templateParameters.Add("configurationMode", configurationMode);
            templateParameters.Add("configurationModeFrequencyMins", configurationModeFrequencyMins);
            templateParameters.Add("refreshFrequencyMins", refreshFrequencyMins);
            templateParameters.Add("rebootNodeIfNeeded", rebootFlag);
            templateParameters.Add("actionAfterReboot", actionAfterReboot);
            templateParameters.Add("allowModuleOverwrite", moduleOverwriteFlag);

            // invoke the New-AzureResourceGroupDeployment cmdlet
            using (Pipeline pipe = Runspace.DefaultRunspace.CreateNestedPipeline())
            {
                Command invokeCommand = new Command("New-AzureResourceGroupDeployment");
                invokeCommand.Parameters.Add("Name", deploymentName);
                invokeCommand.Parameters.Add("ResourceGroupName", azureVmResourceGroup);
                invokeCommand.Parameters.Add("TemplateParameterObject", templateParameters);
                invokeCommand.Parameters.Add("TemplateFile", Constants.TemplateFile);

                pipe.Commands.Add(invokeCommand);

                pipe.Commands.Add("Out-Default");

                Collection<PSObject> results = pipe.Invoke();
            }
        }

        #endregion

        #region compilationjob

        public Model.CompilationJob GetCompilationJob(string resourceGroupName, string automationAccountName, Guid Id)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                var job = this.automationManagementClient.CompilationJobs.Get(resourceGroupName, automationAccountName, Id).DscCompilationJob;
                if (job == null)
                {
                    throw new ResourceNotFoundException(typeof(Job),
                        string.Format(CultureInfo.CurrentCulture, Resources.CompilationJobNotFound, Id));
                }

                return new Model.CompilationJob(resourceGroupName, automationAccountName, job);
            }
        }

        public IEnumerable<Model.CompilationJob> ListCompilationJobsByConfigurationName(string resourceGroupName, string automationAccountName, string configurationName, DateTimeOffset? startTime, DateTimeOffset? endTime, string jobStatus)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                IEnumerable<AutomationManagement.Models.DscCompilationJob> jobModels;

                if (startTime.HasValue && endTime.HasValue)
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response =
                                this.automationManagementClient.CompilationJobs.List(
                                    resourceGroupName,
                                    automationAccountName,
                                    new AutomationManagement.Models.DscCompilationJobListParameters
                                    {
                                        StartTime = FormatDateTime(startTime.Value),
                                        EndTime = FormatDateTime(endTime.Value),
                                        ConfigurationName = configurationName,
                                        Status = jobStatus,
                                    });
                            return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                        });
                }
                else if (startTime.HasValue)
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                         skipToken =>
                         {
                             var response =
                                  this.automationManagementClient.CompilationJobs.List(
                                     resourceGroupName,
                                     automationAccountName,
                                       new AutomationManagement.Models.DscCompilationJobListParameters
                                       {
                                           StartTime = FormatDateTime(startTime.Value),
                                           ConfigurationName = configurationName,
                                           Status = jobStatus
                                       });
                             return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                         });
                }
                else if (endTime.HasValue)
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response =
                                this.automationManagementClient.CompilationJobs.List(
                                    resourceGroupName,
                                    automationAccountName,
                                    new AutomationManagement.Models.DscCompilationJobListParameters
                                    {
                                        EndTime = FormatDateTime(endTime.Value),
                                        ConfigurationName = configurationName,
                                        Status = jobStatus,
                                    });
                            return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                        });
                }
                else
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response = this.automationManagementClient.CompilationJobs.List(
                                resourceGroupName,
                                automationAccountName,
                                new AutomationManagement.Models.DscCompilationJobListParameters
                                {
                                    Status = jobStatus,
                                    ConfigurationName = configurationName
                                });
                            return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                        });
                }

                return jobModels.Select(jobModel => new Commands.Automation.Model.CompilationJob(resourceGroupName, automationAccountName, jobModel));
            }
        }

        public IEnumerable<Model.CompilationJob> ListCompilationJobs(string resourceGroupName, string automationAccountName, DateTimeOffset? startTime, DateTimeOffset? endTime, string jobStatus)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                IEnumerable<AutomationManagement.Models.DscCompilationJob> jobModels;

                if (startTime.HasValue && endTime.HasValue)
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response =
                                this.automationManagementClient.CompilationJobs.List(
                                    resourceGroupName,
                                    automationAccountName,
                                    new AutomationManagement.Models.DscCompilationJobListParameters
                                    {
                                        StartTime = FormatDateTime(startTime.Value),
                                        EndTime = FormatDateTime(endTime.Value),
                                        Status = jobStatus,
                                    });
                            return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                        });
                }
                else if (startTime.HasValue)
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                         skipToken =>
                         {
                             var response =
                                  this.automationManagementClient.CompilationJobs.List(
                                       resourceGroupName,
                                       automationAccountName,
                                       new AutomationManagement.Models.DscCompilationJobListParameters
                                       {
                                           StartTime = FormatDateTime(startTime.Value),
                                           Status = jobStatus,
                                       });
                             return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                         });
                }
                else if (endTime.HasValue)
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response =
                                this.automationManagementClient.CompilationJobs.List(
                                    resourceGroupName,
                                    automationAccountName,
                                    new AutomationManagement.Models.DscCompilationJobListParameters
                                    {
                                        EndTime = FormatDateTime(endTime.Value),
                                        Status = jobStatus,
                                    });
                            return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                        });
                }
                else
                {
                    jobModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response = this.automationManagementClient.CompilationJobs.List(
                                resourceGroupName,
                                automationAccountName,
                                new AutomationManagement.Models.DscCompilationJobListParameters { Status = jobStatus });
                            return new ResponseWithSkipToken<AutomationManagement.Models.DscCompilationJob>(response, response.DscCompilationJobs);
                        });
                }

                return jobModels.Select(jobModel => new Model.CompilationJob(resourceGroupName, automationAccountName, jobModel));
            }
        }

        public Model.CompilationJob StartCompilationJob(string resourceGroupName, string automationAccountName, string configurationName, IDictionary parameters)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                var createJobParameters = new DscCompilationJobCreateParameters()
                {
                    Properties = new DscCompilationJobCreateProperties()
                    {
                        Configuration = new DscConfigurationAssociationProperty()
                        {
                            Name = configurationName
                        },
                        Parameters = this.ProcessConfigurationParameters(resourceGroupName, automationAccountName, configurationName, parameters)
                    }
                };

                var job = this.automationManagementClient.CompilationJobs.Create(resourceGroupName, automationAccountName, createJobParameters);

                return new Model.CompilationJob(resourceGroupName, automationAccountName, job.DscCompilationJob);
            }
        }

        public IEnumerable<Model.JobStream> GetDscCompilationJobStream(string resourceGroupName, string automationAccountName, Guid jobId, DateTimeOffset? time, string streamType)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                var listParams = new AutomationManagement.Models.JobStreamListParameters();

                if (time.HasValue)
                {
                    listParams.Time = this.FormatDateTime(time.Value);
                }

                if (streamType != null)
                {
                    listParams.StreamType = streamType;
                }
                else
                {
                    listParams.StreamType = StreamType.Any.ToString();
                }

                var jobStreams = this.automationManagementClient.JobStreams.List(resourceGroupName, automationAccountName, jobId, listParams).JobStreams;
                return jobStreams.Select(stream => this.CreateJobStreamFromJobStreamModel(stream, resourceGroupName, automationAccountName, jobId)).ToList();
            }
        }

        #endregion

        #region node configuration
        public Model.NodeConfiguration GetNodeConfiguration(string resourceGroupName, string automationAccountName, string nodeConfigurationName, string rollupStatus)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                var nodeConfiguration = this.automationManagementClient.NodeConfigurations.Get(resourceGroupName, automationAccountName, nodeConfigurationName).NodeConfiguration;
                if (nodeConfiguration == null)
                {
                    throw new ResourceNotFoundException(typeof(NodeConfiguration),
                        string.Format(CultureInfo.CurrentCulture, Resources.NodeConfigurationNotFound, nodeConfigurationName));
                }

                string computedRollupStatus = GetRollupStatus(resourceGroupName, automationAccountName, nodeConfigurationName);

                if (string.IsNullOrEmpty(rollupStatus) || (rollupStatus != null && computedRollupStatus.Equals(rollupStatus)))
                {
                    return new Model.NodeConfiguration(resourceGroupName, automationAccountName, nodeConfiguration, computedRollupStatus);                            
                }
                
                return null;
            }
        }

        public IEnumerable<Model.NodeConfiguration> ListNodeConfigurationsByConfigurationName(string resourceGroupName, string automationAccountName, string configurationName, string rollupStatus)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                IEnumerable<AutomationManagement.Models.DscNodeConfiguration> nodeConfigModels;

                nodeConfigModels = AutomationManagementClient.ContinuationTokenHandler(
                    skipToken =>
                    {
                        var response = this.automationManagementClient.NodeConfigurations.List(
                            resourceGroupName,
                            automationAccountName,
                            new AutomationManagement.Models.DscNodeConfigurationListParameters
                            {
                                ConfigurationName = configurationName
                            });
                        return new ResponseWithSkipToken<AutomationManagement.Models.DscNodeConfiguration>(response, response.DscNodeConfigurations);
                    });

                var nodeConfigurations = new List<Model.NodeConfiguration>();
                foreach (var nodeConfiguration in nodeConfigModels)
                {
                    string computedRollupStatus = GetRollupStatus(resourceGroupName, automationAccountName, nodeConfiguration.Name);
                    
                    if (string.IsNullOrEmpty(rollupStatus) || (rollupStatus != null && computedRollupStatus.Equals(rollupStatus)))
                    {
                        nodeConfigurations.Add(new Model.NodeConfiguration(resourceGroupName, automationAccountName, nodeConfiguration, computedRollupStatus));
                    }
                }

                return nodeConfigurations.AsEnumerable<Model.NodeConfiguration>();
            }
        }

        public IEnumerable<Model.NodeConfiguration> ListNodeConfigurations(string resourceGroupName, string automationAccountName, string rollupStatus)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                IEnumerable<AutomationManagement.Models.DscNodeConfiguration> nodeConfigModels;

                nodeConfigModels = AutomationManagementClient.ContinuationTokenHandler(
                        skipToken =>
                        {
                            var response = this.automationManagementClient.NodeConfigurations.List(
                                resourceGroupName,
                                automationAccountName,
                                new AutomationManagement.Models.DscNodeConfigurationListParameters());

                            return new ResponseWithSkipToken<AutomationManagement.Models.DscNodeConfiguration>(response, response.DscNodeConfigurations);
                        });

                var nodeConfigurations = new List<Model.NodeConfiguration>();
                foreach (var nodeConfiguration in nodeConfigModels)
                {
                    string computedRollupStatus = GetRollupStatus(resourceGroupName, automationAccountName, nodeConfiguration.Name);

                    if (string.IsNullOrEmpty(rollupStatus) || (rollupStatus != null && computedRollupStatus.Equals(rollupStatus)))
                    {
                        nodeConfigurations.Add(new Model.NodeConfiguration(resourceGroupName, automationAccountName, nodeConfiguration, computedRollupStatus));
                    }
                }

                return nodeConfigurations.AsEnumerable<Model.NodeConfiguration>();
            }
        }

        #endregion

        #region privatemethods
        /// <summary>
        /// Enumerate the list of NodeConfigurations for given configuration - without any rollup status
        /// </summary>
        /// <param name="resourceGroupName">Resource group name</param>
        /// <param name="automationAccountName">Automation account</param>
        /// <param name="configurationName">Name of configuration</param>
        /// <returns>List of NodeConfigurations</returns>
        private IEnumerable<Model.NodeConfiguration> EnumerateNodeConfigurationsByConfigurationName(string resourceGroupName, string automationAccountName, string configurationName)
        {
            using (var request = new RequestSettings(this.automationManagementClient))
            {
                IEnumerable<AutomationManagement.Models.DscNodeConfiguration> nodeConfigModels;

                nodeConfigModels = AutomationManagementClient.ContinuationTokenHandler(
                    skipToken =>
                    {
                        var response = this.automationManagementClient.NodeConfigurations.List(
                            resourceGroupName,
                            automationAccountName,
                            new AutomationManagement.Models.DscNodeConfigurationListParameters
                            {
                                ConfigurationName = configurationName
                            });
                        return new ResponseWithSkipToken<AutomationManagement.Models.DscNodeConfiguration>(response, response.DscNodeConfigurations);
                    });

                return nodeConfigModels.Select(nodeConfigModel => new Commands.Automation.Model.NodeConfiguration(resourceGroupName, automationAccountName, nodeConfigModel, null));
            }
        }

        private string GetRollupStatus(string resourceGroupName, string automationAccountName, string nodeConfigurationName)
        {
            var nodes = this.ListDscNodesByNodeConfiguration(resourceGroupName, automationAccountName, nodeConfigurationName, null);
            foreach (var node in nodes)
            {
                if (node.Status.Equals("Not Compliant") || node.Status.Equals("Failed") || node.Status.Equals("Unresponsive"))
                {
                    return "Bad";
                }
            }

            return "Good";
        }

        private string FormatDateTime(DateTimeOffset dateTime)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:O}", dateTime.ToUniversalTime());
        }

        private IDictionary<string, string> ProcessConfigurationParameters(string resourceGroupName, string automationAccountName, string configurationName, IDictionary parameters)
        {
            parameters = parameters ?? new Dictionary<string, string>();
            IEnumerable<KeyValuePair<string, DscConfigurationParameter>> configurationParameters = this.ListConfigurationParameters(resourceGroupName, automationAccountName, configurationName);
            var filteredParameters = new Dictionary<string, string>();

            foreach (var configParameter in configurationParameters)
            {
                if (parameters.Contains(configParameter.Key))
                {
                    object paramValue = parameters[configParameter.Key];
                    try
                    {
                        filteredParameters.Add(configParameter.Key, paramValue.ToString());
                    }
                    catch (JsonSerializationException)
                    {
                        throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture, Resources.ConfigurationParameterCannotBeSerializedToJson, configParameter.Key));
                    }
                }
                else if (configParameter.Value.IsMandatory)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture, Resources.ConfigurationParameterValueRequired, configParameter.Key));
                }
            }

            if (filteredParameters.Count != parameters.Count)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, Resources.InvalidConfigurationParameters));
            }

            return filteredParameters;
        }

        private IEnumerable<KeyValuePair<string, DscConfigurationParameter>> ListConfigurationParameters(string resourceGroupName, string automationAccountName, string configurationName)
        {
            Model.DscConfiguration configuration = this.GetConfiguration(resourceGroupName, automationAccountName, configurationName);
            if (configuration == null || 0 == String.Compare(configuration.State, RunbookState.New, CultureInfo.InvariantCulture,
                     CompareOptions.IgnoreCase))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.ConfigurationHasNoPublishedVersion, configurationName));
            }
            return configuration.Parameters.Cast<DictionaryEntry>().ToDictionary(k => k.Key.ToString(), k => (DscConfigurationParameter)k.Value);
        }

        private Model.JobStream CreateJobStreamFromJobStreamModel(AutomationManagement.Models.JobStream jobStream, string resourceGroupName, string automationAccountName, Guid jobId)
        {
            Requires.Argument("jobStream", jobStream).NotNull();
            Requires.Argument("resourceGroupName", resourceGroupName).NotNull();
            Requires.Argument("automationAccountName", automationAccountName).NotNull();
            Requires.Argument("jobId", jobId).NotNull();
            return new Model.JobStream(jobStream, resourceGroupName, automationAccountName, jobId);
        }

        #endregion
    }
}