# Time Logger
If you are required to use the time logging feature of JIRA, and you (as myself) don't like to waste time waiting for your favorite browser to load the ticket, figure out when you have last logged the time etc., and you would rather quickly do it from the keyboard, then this small utility might come in handy!

The utility sits in the background listening for you to invoke it by a global shortcut. Once opened, it asks you to input the ticket number and optionally a comment.

It has been developed for and tested on **Windows only**.

**Key benefits**:
- Maintains the last logged time and does all the necessary calculations on logging so that you don't have to
- Maintains the local cache of what you are logging to retry them in case your JIRA temporarily goes down
- Fetches the ticket summary when you input the ticket to make sure you log for the right ticket
- Keeps track of how much time you have logged for the day and displays it to you on the logging screen

## Building
- Clone the repository (make sure submodules are cloned as well)
- Open the file `TimeLog.sln` in Visual Studio 2019 (or newer)
- Build and run

## Configuration and workflow
Please see [details here](DETAILS.md)

## Roadmap
- [ ] Pull the worklog changes from JIRA periodically
- [ ] Update worklog time from the grid
- [ ] Show worklogs for past dates
- [ ] Abstract the logger from JIRA (i.e. allow it to work with other services)
- [ ] Add support of plugins to support personalized automated time logging flows etc.
- [ ] Add support of Linux and MacOS (only if there is an interest in those versions)

## License
Copyright (C) 2022  Igor Krushch
 
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
 
This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.
 
You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
