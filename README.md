# MinigameNetcode

**Connection Approval** - Now it only allows 4 players, closes application for denied connections. 

**Input IP Address** - Host + clients can enter IP address in-game which allows LAN stuff without having to hardcode the IP in the `NetworkManager`. The input box is ugly rn. Could honestly just remove this for now to make local testing easier. 

**Scrap Player Colors** - The Scrap players are colored like the Platformer players now. 

**Removed Network Manager in Platformer Scene** - Don't *think* we need it since we've got the NetworkManager from the Lobby/Scrap scene. I don't think it broke anything? 
