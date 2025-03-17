# Learner Data Api

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

Holds information on Learner Data.

## How It Works

This web api serves as the inner api for the Learner Data stack of services:

## 🚀 Installation

### Pre-Requisites

The Azure Functions component is not necessary for the website to function but can be found [here](https://github.com/SkillsFundingAgency/das-levy-transfer-matching-functions)

* A clone of this repository
* A code editor that supports .Net 8.0
* The latest [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config) for:
  *  `SFA.DAS.LearnerDataApi_1.0`

### Config


This utility uses the standard Apprenticeship Service configuration. All configuration can be found in the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) which may be more up-to-date than what is described here.

Azure Table Storage config

Row Key: SFA.DAS.LearnerData.Api_1.0

Partition Key: LOCAL

Data:

```json
{
  "LearnerDataApi": {
    "DatabaseConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.LearnerData.Database;Integrated Security=True",
    "NServiceBusConnectionString": "UseDevelopmentStorage=true",
    "NServiceBusLicense": "",
    "DataProtectionKeysDatabase": ""
   },
"AzureAd": {
    "tenant": "citizenazuresfabisgov.onmicrosoft.com",
    "identifier": "https://citizenazuresfabisgov.onmicrosoft.com/das-at-ldapi-as-ar"
  }
}
```

## Technologies

* .NetCore 8.0
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions

## 🐛 Known Issues

* This web api must be run under the Kestrel web server
