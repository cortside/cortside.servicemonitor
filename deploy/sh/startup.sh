#!/bin/bash

ifconfig
cp $LOCAL_APPSETTINGS appsettings.local.json

./Cortside.HealthMonitor.WebApi
