# 4+1 Architectural Model Documentation

## Table of Contents
- [1. Logical View](#1-logical-view)
- [2. Physical View](#2-physical-view)
- [3. Deployment View](#3-deployment-view)
- [4. Scenario View](#4-scenario-view)
- [5. Summary](#5-summary)
- [6. Glossary](#6-glossary)

---

## 1. Logical View
**Purpose**: Provides an overview of the system's logical structure, with a focus on functionality and organization of the code.

- **Description**:
    - Outline the system's primary modules and their relationships.
    - Describe key subsystems, layers, and packages.
    - Detail how responsibilities are distributed within modules and how they interact.

#### Level 1

![](Views-SEM5PI/Logical%20Views/Level1/View.png)

#### Level 2

![](Views-SEM5PI/Logical%20Views/Level2/View.svg)

#### Level 3 (Backoffice)

![](Views-SEM5PI/Logical%20Views/Level3/Backoffice/View.svg)

#### Level 3 (PMD - Patient Medical Data)

![](Views-SEM5PI/Logical%20Views/Level3/PMD/View.svg)


#### Level 3 (Frontend)

![](Views-SEM5PI/Logical%20Views/Level3/Frontend/View.svg)

### Key Elements
- **Modules**:
    - *Frontend*: The frontend component of the system, dedicated to provide smooth user experience
    - *Backoffice*: The backoffice component, dedicated to handling business logic and technological functionality of the system.
    - *Scheduling:* The scheduling component, dedicated to optimization of business logic handling. More specific informations regarding this module are to be defined in future sprints.
- **Relationships**:
    - Frontend module will provide the interface for the users of the platform to interact with it in a safe and controlled manner
    - Backend modules (Scheduling and Backoffice) wilsl have contact established with the exterior through their provided APIs that will be "consumed" by the frontend module

---

## 2. Physical View
**Purpose**: Illustrates the physical hardware components and how software components are mapped onto them.


### Level 2

![](Views-SEM5PI/Physical%20Views/Level2/View.svg)

- **Description**:
    - The user will interact with the system through the browser accessing the frontend module of the application
    - The frontend will communicate with the Server that contains the backend modules Backoffice and Scheduling
    - Level 1 and 3 physical views were deemed unnecessary considering a layer 1 view would not be explicative at all and a layer 3 view as components in the same device are not in separate containers
  
### Key Elements
- **Devices**:
    - *EndHost*: The machine used by the user to connect through HTTP requests to the Server
    - *Server*: The machine that the system will be ran on and receives HTTP requests.
- **Connections**:
    - The system will connect and "converse" with the user using the HTTP/s protocol

---

## 3. Deployment View
**Purpose**: Details the environment setup, including server configurations, containers, and cloud services.

### Level 1
![](Views-SEM5PI/Deployment%20Views/Level1/Deployment-View-Sprint1-Level1.svg)

### Level 2
![](Views-SEM5PI/Deployment%20Views/Level2/Deployment-View-Sprint1-Level2.svg)

### Level 3
![](Views-SEM5PI/Deployment%20Views/Level3/Deployment-View-Sprint1-Level3.svg)



- **Description**:
    - In the 3 levels of the deployment layer we can see the organization of software components and their relationships within the development environment.
    - The system is designed to follow a layered approach along the Onion module


### Key Elements
- **Environment**:
    - *Environment Name*: On premises deployment, system is hosted on a machine by the team accessed remotely by the users.
- **Layers**:
  
  - **Infrastructure Layer:** Infrastructure that provides the technology for technology communication
  - **Interface Adapters Layer:** Adapter Layer that allows system to communicate with the infrastructure technologies
  - **Domain Services Layer:** Domain Layer Services that are the entry and exit points for the application, allow interior business logic containing objects to contact outside instructions
  - **Domain Layer:** Domain Layer objects that contain business rules, the deepest and most inner layer of the application


- **Services**:
  - App Services: Application services dedicated to system orchestration and manipulation
  - Domain Layer Services: Domain Services related to business logic.

- **Deployment Pipeline**:
    - Continuous Integration and Deployment procedures are implemented through the bitbucket repository configuration with the [pipeline configuration files](../../../bitbucket-pipelines.yml) that enforces good continuous integration and deployment practices.

---

## 4. Scenario View
**Purpose**: Demonstrates the relationships between business procedures and the actors responsible for carrying them out.

### Use Case Diagram

![](Views-SEM5PI/Scenario%20View/Scenario-View-Sprint1.svg)

**Note:** More specific information regarding the functioning of each business procedure is detailed in each according user story's process view.


---



# Relevant Mappings

## Logical - Deployment

### Layer 2

![](Views-SEM5PI/Mappings/Logical-Deployment/View.svg)


### Layer 3

![](Views-SEM5PI/Mappings/Logical-Deployment/L32-Deployment-Logic-Map.svg)

## Physical - Deployment

![](Views-SEM5PI/Mappings/Physical-Deployment/Deployment-Physical-View.drawio.svg)

## 5. Glossary

[Glossary File](Glossary.md)
