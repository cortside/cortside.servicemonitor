#!/bin/bash

ifconfig
cat $LOCAL_APPSETTINGS > appsettings.local.json

./Cortside.HealthMonitor.WebApi
