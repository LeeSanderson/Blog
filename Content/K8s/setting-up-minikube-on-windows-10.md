---
title: Setting up minikube on Windows 10
abstract: Instructions to help you set up minikube on a Windows 10 Home edition
date: 2019-06-26
tags: minikube,k8s
---
# Setting up minikube on Windows 10

These instructions will help you set up minikube on a Windows 10 Home edition.

These instructions will also work with Windows 10 Professional and Enterprise editions but you must first disable Hyper-V support (via "Programs and Features" in Control Panel).

1. You may need to enable virtualization (this usually involves making some changes to the BIOS). To check whether this step is required you can use tools like [Speccy](https://www.ccleaner.com/speccy).

1. Install [chocolaty](http://chocolatey.org/) - this will simplify the rest of the install process.


1. Open an admin cmd console or PowerShell console.
Run "choco install virtualbox" in the console to install the Virtual Box type 2 hypervisor required to run Docker.

1. Run"choco install docker-toolbox" to install a legacy docker, docker-machine, docker-compose, etc. This is required as the newer docker desktop is only supported on Windows 10 64bit: Pro, Enterprise or Education (Build 15063 or later).

1. Double click on the "Docker Quickstart Terminal" shortcut (this should appear on the Desktop after docker toolbox is installed). This will setup and configure docker on the local machine

1. Run "choco install minikube" to install minikube.

1. Run "minikube start" to start minikube and double-check your configuration - this may take some time on the first run.

1. Ensure that minikube is running correctly and kubectl has been configured to talk to minikube by running the following commands: minikube status, kubectl config view, and kubectl cluster-info.

