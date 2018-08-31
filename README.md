# Hexagonal Architecture
Purpose for this repository was to show the concept of Hexagonal Architecture by Alistair Cockburn as simple as possible.
Which in my understanding is not about architecting the software per se but about the way of thinking on software development.

### Structure
Solution consists of three projects:
* Printer - a hexagon
* Printer.Test - a test client of the hexagon
* PrinterSample - a console client of the hexagon

### Prerequisites
* Docker
* RabbitMq Server (*only for full local deployment of PrinterSample*)

### Installing
#### Printer
To build a Printer project use either **build.ps1** or **build.sh**. This will publish the project to the **artifacts**.

#### PrinterSample
To build a PrinterSample project use a **sample.ps1**. Script is taking a one of three parameters
* local - for fully local deployment
* docker - for fully dockerized deployment
* partial - for deploying PrinterSample locally and dependant services into Docker

As an example for fully dockerized deployment run following command in Powershell
```powershell
    .\sample.ps1 docker
``` 