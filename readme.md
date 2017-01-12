# WhereBot

A C# REST API to help locate people and resources in a hot-desking environment

### Overview

In a large organisation you might not know where everyone sits or where your nearest printer is, and it's even more tricky if you work in a hot-desking environment where you might sit at a different desk (or floor, or building) every day.

WhereBot helps with this by tracking the locations of people and resources...


```
@wherebot set my location [mydesk_name]
```

... and then lets you find them on a floorplan ...


```
@wherebot where is @mycolleague
@wherebot where are @mycolleague @myboss "colour printer"
```

WhereBot consists of a headless REST API, and currently exposes features through a Slack bot, but as it's just a REST API you can easily write you own UI or bot in whatever you want to use.

We're currently in a very early prototype, but the above WhereBot commands are already implemented.

#### Admin Tasks

We're working on building out the bulk of the admin commands, but you can still do the following if you know how to use a text editor:

* Add a floorplan image
* Add and edit locations
* Assign other people and shared resources to locations

#### Future Work

These are other things we think would be super-useful, but aren't aailable right now:

* Support multiple floorplans
* Slack bot commands for managing floorplans and locations
* Web UI for setting your location and searching
* Assign people and resources to groups, and show the locations of entire groups