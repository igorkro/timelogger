# Time Logger
Copyright © 2022 Igor Krushch

The utility to simplify a JIRA time logging.

Please email bug reports and feature requests to mailto:dev@krushch.com


## Configuration
There are a few things we need to configure before we can use it.

- **Authentication**. Pick the one based on what mode allowed by JIRA instance. Access Token is a preferred one though, as it is a bit more secure.
- **Misc ticket**. We will inevitably work on some things that are not ticketed up (helping QA, debugging some prod issue, have a meeting etc.). For those things we have a dedicated ticket, misc ticket. It will be assigned to you, will be there the whole time in the Proposed state, no code is going to be committed in there - it is strictly to log some unticketed work. If you don't have that one already, please create one, and put the ticket key into this field.
- **Jira URL**. This is the URL that JIRA resides on (for example, https://jira.testdomain.com/)
- **Command Shortcut**. This is the global shortcut that will open the logging window.
- **Show Total Logged on Command Screen**. Turn this one on to display the total logged time for the day on the Command screen.

Press Apply.

### Access Token Setup
If you decide to use the access token, you need to do the following.
Open JIRA and press on your avatar in the right top corner. From the drop-down menu select "Profile".
On the left hand side select "Personal Access Tokens" and press Create Token. Give it a name. A good practice to give the name that allows to identify the target app, thus call it "Timelogger". Untick "Automatic expiry" to make sure token never expires and press Create. You will be presented with the token value. Copy that value. Note that this is the only time you will be shown this token! So if you forgot it, you will have to recreate the token!

Go to the logger configuration, select "Access Token" , paste the value copied from JIRA and press Apply.

## The workflow
The application should start up automatically on signing in if you have selected it to run automatically on startup.

The first thing when you start working is log the start time. To do that press Ctrl+Alt+Space (or whatever Command Shortcut you have assigned).

The logging screen is very simple - it contains the text box for providing logging instructions, and the Last Reported that shows when the last time you have logged something for the day.

If you open the logged and the Last Reported time is "`---`", that means you didn't log anything and didn't mark the day start. If you try to log when there are dashes, it will prompt you to specify when did you start working today so that it can calculate logged time properly. However, if you have opened the logger to report the start of the day, the dialog won't show up.

To do that input dash (`-`) in the logger window and press Enter. It will ask if you want to log this entry. Press Yes.

Dash is a special command that moves last logged time pointer (or the start of the work for that matter) to the current point in time.

NOTE. On startup when you open the logger for the first time, the command text box won't be focused. You need to explicitly click on it. After that opening the logger window will keep command text box focused, thus you will be able to input log entry right after the Command Shortcut press.

Then you do some work, check emails etc. When you are switching between activities (lets say finished checking emails and switching to the task), you again press your Command Shortcut and input `emails` and press Enter. What this will do is it will log this time into the Misc ticket, and the time taken will be difference between your last logged time and current moment. 

Then, say, you have worked on a ticket for a little while. Lets say the ticket is `ECI-212`. You again press Ctrl+Alt+Space, input `ECI-212` and press Space. The logger will fetch the name of the ticket and display it underneath the text box. This is just a visual help to make sure you log under the right ticket. After that space input a short description of what you have done (lets say, coding). And press Enter. This will log another entry.

Lets say you then went for a lunch. You just go, have a lunch, and then when you come back you again Ctrl+Alt+Space and input dash. That will make sure that it skips the time you have been absent.

Then you again worked on `ECI-212` for a few moments. You press again Ctrl+Alt+Space. You can now either input `ECI-212` again and press Space, or you can press Down arrow. This will display the list of tickets you have logged for since the app start. You can select the `ECI-212` there and press Enter. It will put the ticket in to the text field. You then again input some details and press Enter. If there is no ticket that you need in that drop down list, press Escape to close it and input the ticket manually.

Alternative to using that "dash" log to move the last logged time pointer, you can right click on the Logger icon in the tray area and press "Notify".


## Special commands supported by the logger

### Manual time logging
`+hh:mm-hh:mm <ticket-id> <details>`

where the first `hh:mm` denotes the work start time, and the second `hh:mm` denotes the work end time

### Report with time offset
`<ticket-id> <details> | <hh:mm>`

record the time with the offset from the left (i.e. from the time started).

`hh:mm` as a positive value will "move" the start time to be prior to last reported time, hh:mm as a negative value (`-hh:mm`) will "move" the start time to be after the last reported time


### Display time table
`!list`

This command shows the time table. Alternatively you can right click on the Logger tray icon and select "Time table..." there.

The window shows your logged entries for the day. 
Red vertical line shows when you have logged the dash (i.e. the mark when to start logging from).
Blue vertical line shows the current time.

Pressing on the log entry will display you the details (the logged details and the time period).
You can also press the ticket name on the right, it will open the ticket in the browser.

At the bottom you can see some basic stats:
- **Total reported**. That is how much time has been logged (that is counting ticket work and misc)
- **Last reported**. This just shows when was the last time you have logged time for the day

The time table only supports times from 6am till midnight. You are not supposed to work at night, hence the logger doesn't support that presently.


### Display unflushed entries
`!unflushed`

This command shows the dialog with entries that failed flushing to JIRA


### Display config screen
`!config`

This command shows the settings screen


## Fail tolerance
You will notice in the time table that all the entries have a small green square at the bottom of the entry's rectangle. This square denotes that the entry has been successfully propagated to JIRA.

There are cases where due to JIRA maintainance or network failure the logger won't be able to send the entry to JIRA (or sending fails).

To resolve that issue, the logger maintains logged entries locally with the status of the flush success. If the entry wasn't flushed, then in the time table you will see a small red rectangle instead of a green one. 
The logger will attempt to flush those pending entries every time you try to log something. Alternatively, if there is JIRA inaccessibility detected and some entries are not flushed, you can right click on the logger 
icon in the tray area, and will see "Retry" menu item. You can press it for logger to retry the flushing manually.

You can also see what entries have failed to be flushed and can attempt to individually flush them.

To do that open the logger by pressing Ctrl+Alt+Space and input
`!unflushed`

This will display a dialog with the list of failed entries. You can select what date you want to see failed entries for (default is today). You can then select entries of interest and press "Flush selected". 
The dialog will then update depending on the result of flushing.
