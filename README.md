# Secret Santa

This project is a web application that allows users to organize Secret Santa events with their friends. Secret Santa is an gift exchange (usually done for Christmas) where each member of a group are assigned to another member to provide a gift anonymously. My friends from high school and I have a yearly Secret Santa exchange, however, because we all live in different states, we always have difficulties organizing the event. Although we've found online Secret Santa event organizers, none of them seem to work as well as we would want it to (e.g., the dashboard isn't well organized, not very user friendly, etc). So, I've decided to make my own version of the Secret Santa web application
<br />
<br />
[Click here to check it out!](ec2-18-191-204-241.us-east-2.compute.amazonaws.com)
<br />
## About
* ASP.NET Core based web application structured via MVC that allows users to organize Secret Santa events
* RESTful routing architecture
* Features random Secret Santa gnerator that assigns each member of the event to another member
* Technologies and Languages used: C#, ASP.NET Core, MySQL Workbench, HTML, CSS, Bootstrap
* Deployed using AWS EC2
## Get Started
1. An user can first sign up to create an account. Each user will have a unique email address, and thus cannot register if the same email is already in use. After signing up, or signing in, the user will be taken to their dashboard page. If signed in, the user will be automatically taken to the dashboard page from the root route. The dashboard page displays Invites (where the user can accept an invite to an Event), My Events (where you can click and view the events the user is apart of), and Organized Events (where the can edit Events that he/she created).
1. To add a new event, you can click 'Add Event' on the top right side of the nav bar. When a new event is created, the user will be redirected to the dashboard where he/she can edit the event to invite other users.
1. When the edit form is submitted, the invitations will be sent. Each invited user must accept their invite.
1. Once everyone has accepted their invites, the "Generate Secret Santa" button will appear on the Organizer's edit event page. The Organizer can then click the button, which will randomly assign each member another member of Event to gift. Once the Secret Santas are generated, the Organizer cannot invite any new members to the event.
1.Each member of the event can view who they were appointed to through the Event page in the My Event section of the dashboard. Each user has a 'My Wishlist, where he/she can add item names of things they want to give their Secret Santa an idea of what he/she wants.
