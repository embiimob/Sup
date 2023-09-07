# Sup!? 
experimental decentralized state engine browser

Sup!? is a demonstration of establishing a decentralized social identity and tracking state changes with public and private comments on immutable blockchain objects in a format familiar to NFT enthusiasts. The experimental Sup!? object browser is currently hardcoded to reference the bitcoin testnet mainchain. A production deployment will be released December 2023. Our goal is to give #teamworld ample time for testing and integrations.

This experiment is using an ancient blockchain protocol called P2FK (Pay to Future Key) invented by http://HugPuddle.org in 2013.

P2FK was directly inspired by the satoshi uploader  see --> https://cirosantilli.com/satoshi-uploader

 As of Sup?'s first release date, all transactions and objects discoverable in the experiment were created by embii using http://apertus.io

## **Installation**

1. Download the Sup!? v0.1.56-beta.zip file (https://github.com/embiimob/Sup/releases/download/Supv0.1.56-beta/Sup.v0.1.56-beta.zip)
2. Create a folder on your **fastest** disk drive with at least **50GB** free (it will be storing the bitcoin testnet blockchain)
3. Unzip all contents into the folder
4. Create a shortcut to SUP.exe and launch

**You will need to sync a blockchain to perform your first Sup!? object search**
1. Click the key button
2. Launch a full bitcoin testnet mainchain node using the button provided
3. Launch IPFS using the button provided "enable IPFS pinning"
4. Wait until testnet is fully synced. ( this part will take several hours )
5. Type #flowersofwar in the main search box and hit enter
<br />
<br /> 

## **NOTICE**
Sup!? is an experimental tool made by adults for #adults. Sup!? is not suitable for children. 
<br />
<br />
## **Search Examples**

### **muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs**
searches mainchain - bitcoin testnet by address returns all object associations and the associated profile

### **6d14b0dc526a431f611f16f29d684f73e6b01f0a59a0b7b3d9b8d951091c2422/index.html**
searches mainchain - bitcoin testnet by transaction id returns referenced index.htm(l) in browser

### **BTC:0618f12af65a4e82f8e7b41f8578721dfeb109e9a73ff71aebdbc982696e3720/index.html**
searches sidechain - bitcoin production by transaction id returns referenced index.htm(l) in browser

### **MZC:4dbb0e984586d1994f461c419c460edf7ecf15488a8b11282f19cec9aa7ec285/index.html?viewer=embii4u**
searches sidechain - mazacoin production by transaction id returns referenced index.htm(l) in browser

### **#CaseSensitiveKeyword**
searches mainchain for up to a 20 character case sensitive keyword and shows all public message and object associations

### **@UserName**
searches mainchain by urn - shows currently registered profile object links associated with @username and shows all object associations. profile registrations are transferable and expire after 3 years of no activity allowing them to be claimed by other registrants in the future.

### **ipfs://QmU42aLJToYmshwJu26iprH9RsX6SfJDw3FTf6senJEdF1**
performs a ipfs get, archives ipfs file(s) locally, pins file(s) if pinning is enabled, and opens a system folder dioluge allowing you to explore any files found

### **sup://twitter.com**
searches mainchain by urn - shows currently registered object with uri redirection.  urn registrations are transferable and expire after 3 years of inactivity allowing them to be claimed by other registrants in the future.  ( there are no urn character limits, uri redirects are NOT enabled )

### **http(s)://embii.org**
navigates to requested url in browser.
<br />
<br />
## **Public / Private Messaging / Social / #Keyword Search / GIF Selector / Walkie Talkie **
The disco ball has dropped you can now send direct public and private messages using Sup!?, the bitcoin testnet and #IPFS.   Search for your own profile in the object browser to login with your local account.  Search for aditional profiles and add them as friends by clicking the follow button.  Click on a friend's icon to show their latest public messages. Click on the community icon to see a combination of all your friends posts in a single feed in the order that they were confirmed on the blockchain. Sup!? search also now finds and displays all public messages by #keyword.  Click on the 📢 to send the profile you are currently browsing a public message.  Click on the 🤐 before clicking on the 📢 to send the profile you are currently browsing a private message. Click the GIF button to search through a growing collection of Gifs etched into the blockchain by other Sup!? users. Click and hold the new audio record button and your audio message will be delivered imediately after letting go of the record button ( if walkie talkie mode is enabled in the connection screen ).  Search for your own profile and click on the 🤐 icon to see any private messages sent to it.
<br />
NOTE: all attachments in your private message are encrypted via Sup!? using the recipients public keys before they are uploaded to IPFS for delivery.
<br />
## **JukeBox 🎵**
Click the new 🎵 and the currently active profile will be searched for mp3 and wav files, if found they will be displayed in an ordered list and played one after the other. You can also perform #Keyword searches and all audio files found at that #keyword will be displayed and played. it is also possible to search by profile urn or address.  click on any link to skip to it. scroll to load additional audio cliips.
<br />
search for experimental to hear a collection of 2007 - 2009 embii audio experiments.
<br />
## **Profile Minting / Editing**
Click the mint button and select the profile mint button. Enter a urn and a profile address or click the diamond button to generate a new profile address. These two fields are all that are required to register a profile urn. You may optionally enter a PFP image url, your full name and suffix, web links, contact information and keywords. NOTE: Every profile becomes an entry point for which private messages may be delivered as every profile displays public keys to be used for Secp256k1 encrypted messaging.   
<br />
Once you are satisfied with your profile information. click mint. Your profile will take effect following a successfull confirmation of the transaction.
<br />
To edit a profile, click on the magnifying glass and enter the profile address to edit. If a profile is found at the address, Sup!? will build and display it on the screen. Modifying any of the fields will cause the corresponding fields' modified indicator to turn blue. Remove any data that is not being updated from the transaction by clicking its corresponding button until it is not blue. Click mint when you have completed making updates. Your changes will take effect following a successful confirmation of the transaction.
<br />
Your profile urn registration is valid for 3 years past the last change date. This means that in order for your profile to expire, there must not be a single change transaction placed to its holding address for a period of 3 years. Should this occur, the urn registration will be claimed by the next available registration that falls under the same time restriction. If no claims exist, the profile will no longer be discoverable and can be claimed by anyone.
<br />
<br />
## **Object Minting with Royalties / Printing / Editing / Batch Burning / Batch Giving**
Click the mint button and select the object mint button. Enter an object name, object address, owner address(s) with qty and a urn. These fields are required. Click mint. If no creator address is entered, the object will be self-signed using a new address generated at your minting station wallet. To further enhance your collector's experience, a profile can be setup on a collection address and the collection addresss can be added as a creator. Now, both the collection shortname name and the creator short name will be discoverable in the browser. Searching for the creator shortname returns all objects.  Searching for the collection shortname returns just the collection. in preparation for the future BUY and LST transaction types, royalties can now be defined in the object mint screen. up to 100% of all future sales royalties can be claimed on the roy tab and theoretically divided in to 1000s of recipient addresses in a single p2fk transaction.
<br />
Click print instead of mint and you can use a paper transaction alone to prove ownership. Any person can scan a paper mint & send a small confirmation transaction to the addresses within the QR code printed on the paper to make the paper transaction public. A Sup!? paper transaction can be used in lieu of a public confirmation as it contains all the necessary signature and transaction data necessary to confirm it against a current scan of the object address it is associated with.
<br />
To edit an unlocked object, click on the magnifying glass and enter the object address to edit. If an object is found, Sup!? will build and display the object it represents on screen. Modifying any of the fields will cause the corresponding fields' modified indicator to turn blue.. Click mint Or print when you have completed making updates and your changes will take effect following a successfull confirmation of the transaction.
<br />
<br />
## **Object Listing / Offering / Buying  with in memory pending transaction monitoring**
The Sup!? experiment now offers trustless, service fee free marketplaces for all p2fk based Sup!? objects.  Click on the ⚡️ icon found on the object details screen to open it's new live market window.  all Current listings and non refundable offers will be displayed.  double click on the listing and offer labels to refresh the lists. All in memory transactions will be searched and anything related to the object will be displayed ( LST, BUY and GIV transactions ). following this initial search all new transactions will be displayed within 5 seconds of being broadcast on the blockchain.  in memory transactions will dissapear from the live monitoring list within 5 seconds of the first confirmation.  add a signature address to the Signature address field and enter the qty and each cost and the owner address to buy from if buying. click either the BUY or LIST buttons depending on your desired function.  a cost estimate is displayed.  Click on the Buy Or List buttons again to complete the transaction.  you will be prompted to confirm.

BUY transactions that do not correspond to any current listing will instead create a non refundable offer.  to close an offer the current owner can send any qty of the object to the requestor via a GIV transaction. An offer can be closed without giving anything to the requstors by sending a qty of 0.  (the closing function can be done in bulk across multiple objects via a single bulk GIV transaction)
<br />
<br />
## **Live Monitor**
Click the live button and watch in realtime as other Sup!? users mint their objects and compose public messages. All new objects and messages will be added to the top of your browser window pushing the older objects and messages down the screen. A maximized Sup!? monitor that shows a constant stream of new objects and messages makes for a great display. Sup!? montiors the in-memory pool allowing it to display new objects before they are confirmed in the blockchain. Your Sup!? objects and messages will appear in every curently live Sup!? montior around the world in about 10 - 30 seconds after you have etched the metadata. All blockhains included with this experiment are monitored for new objects and messages if they are running and fully synced.  
<br />
With live monitoring enabled, you never know what might pop into the window. The damage is limited to a thumbnail image and what can fit in a small description text box. Objects that are discovered in live mode are not fully inspected for safety reasons. You can delete one directly from the live preview panel by clicking on the object's small trash icon. This will remove any trace of it from your system. If you are still concerned, clearing all Sup!? caches will without a doubt remove it.  
<br />
Consider running a seperate instance of Sup!? in it's own folder for live monitoring.  You can then easily clear the cache in the live monitoring folder without effecting your overal Sup!? browsing cache.  Launch the blockchain from your normal folder and just run Sup!? live from your second folder..  both instances of Sup!? will communicate to your launched blockchain without any problems.
<br />
<br />
## **Official / Unofficial Registration Indicator**
When a Sup!? urn is registered, a small symbol appears on the top left corner of the object's thumbnail on the main browser screen, replacing the urn label on the object details screen. Following registration, any other Sup!? objects referencing the same urn will display a [ SEE OFFICIAL ] button that redirects them to the officially registered urn. The unofficial urn reference can still be opened by clicking anywhere elese on the object.  
<br />
<br />
## **File Lookup**
Drag and drop any file onto the main Sup!? browser screen. Sup!? will search the mainchain for a current file registration with an owner. If found, the registered object will be returned in the main browser window. All URN's with valid file paths at the time of registration are automatically registered for Sup!? file lookup.
<br />
<br />
## **Block \ Mute \ 🗑️**
Blocking addresses ignores any associations to them going forward, preventing a transaction or any transactions signed by the blocked address from outputting to disk. A purge and reinspection is required to completely eliminate all data. If a group of collectors decides to block an address, they can technically ignore them out of existence.
<br />
Muting an address prevents any further transaction comments signed by the address from outputting onto a disk. A purge and reinspection is required to completely eliminate all transaction comments.
<br />
clicking the 🗑️ icon found in message and object search results deletes them from disk including any thumbnails or attachments they may have been displaying at the time.  it also places a null transaction record into the cache to prevent the object or message from outputting onto a disk in the future. 
<br />
<br />
## **Web Applet Querystring Data**
Trusted index.html and index.htm files are sent the following Sup!? object information via querystring on load:
"address=", "creator=", "viewer=", "viewer-name=", "owner=", "owner-name=", "urn=", "uri=", "url=", "height="
<br />
<br />
## **Command Line Interface**

**get root by transaction id:**
  SUP.EXE --versionbyte 111 --getrootbytransactionid --password better-password --url http://127.0.0.1:18332 --tid 6d14b0dc526a431f611f16f29d684f73e6b01f0a59a0b7b3d9b8d951091c2422 --username good-user<br />
<br />

**get roots by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getrootsbyaddress --password better-password --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get public address by keyword:**<br />
  SUP.EXE --versionbyte 111 --getpublicaddressbykeyword --keyword "20 BYTE ASCII STRING"<br />
<br />

**get keyword by public address:**<br />
  SUP.EXE --address mmw6JJrmsEZ1bwyVPKvfRFwpoJ62nJJCsV --getkeywordbypublicaddress<br />
<br />

**get object by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getobjectbyaddress --password better-password  --url http://127.0.0.1:18332 --username good-user --verbose<br />
<br />

**get object by transaction id:**<br />
SUP.EXE --tid 69ae3a76a9de22ffad7bfb9249824512fc38e01d82e2010877ead179b50f0f77 --versionbyte 111 --getobjectbytransactionid --password better-password --url http://127.0.0.1:18332 --username good-user<br />
<br />


**get object owner by urn registration:**<br />
  SUP.EXE --versionbyte 111 --getobjectbyurn --password better-password --url https://127.0.0.1:18332 --username  good-user --urn twitter.com<br />
<br />

**get object owner by file :**<br />
  SUP.EXE --versionbyte 111 --filepath C:\\folder\\test.jpg --getobjectbyfile --password better-password --url  http://127.0.0.1:18332 --username good-user<br />
<br />

**get objects by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getobjectsbyaddress --password  better-password --url http://127.0.0.1:18332 --username good-user --skip 0 --qty -1<br />
<br />

**get objects owned by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getobjectsownedbyaddress --password  better-password --url http://127.0.0.1:18332 --username good-user --skip 0 --qty -1<br />
<br />

**get objects created by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getobjectscreatedbyaddress --password  better-password --url http://127.0.0.1:18332 --username good-user --skip 0 --qty -1<br />
<br />

**get objects by keyword:**<br />
  SUP.EXE --versionbyte 111 --getobjectsbykeyword --keyword flowersofwar --password better-password --url  http://127.0.0.1:18332 --username good-user --skip 0 --qty -1<br />
<br />

**get found objects:**<br />
  SUP.EXE --versionbyte 111 --getfoundobjects --password better-password --skip 0 --qty -1 --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get keywords by address:**<br />
  SUP.EXE --address mwJDUTXksGKUmU3z9nKeMvnjNnWjEXj5rW --versionbyte 111 --getkeywordsbyaddress --password better-password --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get public messages by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getpublicmessagesbyaddress --password better-password --skip 0 --qty 10 --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get private messages by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getprivatemessagesbyaddress --password better-password --skip 0 --qty 10 --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get public keys by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --getpublickeysbyaddress --password better-password --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get profile by address:**<br />
  SUP.EXE --address muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs --versionbyte 111 --getprofilebyaddress --password  better-password --url http://127.0.0.1:18332 --username good-user<br />
<br />

**get profile by urn:**<br />
  SUP.EXE --versionbyte 111 --getprofilebyurn --password better-password --url http://127.0.0.1:18332 --username  good-user --urn embii4u <br />
<br />

**parameters:**

  --getrootbytransactionid        

  --getrootsbyaddress            

  --getpublicaddressbykeyword     

  --getkeywordbypublicaddress     

  --getobjectbyaddress            

  --getobjectbytransactionid

  --getobjectbyurn                

  --getobjectbyfile               

  --getobjectsbyaddress           

  --getobjectsownedbyaddress      

  --getobjectscreatedbyaddress    

  --getobjectsbykeyword 
  
  -getpublicmessagesbyaddress
    
  -getprivatemessagesbyaddress
  
  -getpublickeysbyaddress

  --getfoundobjects

  --getprofilebyaddress           

  --getprofilebyurn               

  -u, --username                  RPC username for authentication

  -p, --password                  RPC password for authentication

  -r, --url                        RPC URL

  -s, --skip                      The number of roots \ objects to skip [default: 0]

  -q, --qty                       The number of roots \ objects to retrieve per processing batch [ default: 300 \ all ]

  -b, --versionbyte               The version byte

  -t, --tid                       The transaction ID to query

  -a, --address                   The address to query

  -k, --keyword                   The keyword to query

  --urn                           The urn to query

  -f, --filepath                  The full path of file to query

  --verbose                       output event information to leveldb

  --help                          Display this help screen.

  --version                       Display version information.


## **URN IMG & URI Referencing Standards**<br />
<br />
IPFS:Qma7o6Yx2HQfCkNJEHv1gpiDzoZU8rNc6JFQXijfyt8cGc<br />
( references an IPFS gif file creates a default file named {hash}/artifact  )<br />
<br />
IPFS:Qmex6VRDqwVCMPrc7ojtBWzUBPdQAmWq9FcprKTcgmbKX3/baby punch.glb<br />
( references and IPFS file outputs file with given name {hash}/baby punch.glb  )<br />
<br />
66e5f4df4bd0a3ea9b569498ed25f848e837b9dec7a2699c1d6756ae9560c962/8354.png<br />
( references mainchain - bitcoin testnet P2FK )<br />
<br />
MZC:c0c7fa9536e31e04a65252d8acec29af1c54cb0a44609af7fb9e3804065c7f19/chief-sitting-bull.jpg<br />
( references sidechain - mazacoin production P2FK )<br />
<br />
BTC:3ff52882c93420c8fe4a90f6fa94b2a563316b5e7b83afe2ddd5bcadc86d3821/FakeUFO.png<br />
( references sidechain - bitcoin production P2FK )<br />
<br />
LTC:7f698f396cbad1d45ad5a5c474b1492172705ce532dbe1022a7bfe834d914fdf/creationLTCsm.jpg<br />
( references sidechain - litecoin production P2FK )<br />
<br />
DOG:73e146c1b4c1ad9c05de733bbc8c9b682b25b69054492b84c090dd9b1cb0c58f/dodge-meme.gif<br />
( references sidechain - dogecoin production P2FK )<br />
<br />
http://bitfossil.org/7033eb8138de0d3f4be111a57dfb8319b400d3b6a6f5b387a22b334ebb998e93/EMBII.jpg<br /> 
( references a http(s) address )<br />
<br />


- this experiment uses a v0.13.2 full bitcoin core clone with address index change https://github.com/btcdrak/bitcoin/tree/addrindex-0.14

- this experiment uses litecoin core; the following release is included in the deployment https://download.litecoin.org/litecoin-0.16.3/win/

- this experiment uses dogecoin core; the following release is included in the deployment https://github.com/dogecoin/dogecoin/releases/tag/v1.14.6

- this experiment uses mazacoin core; their latest official release has been included in the deployment https://github.com/mazacoin/maza

- this experiment uses oodrive's levelDB; a stable release has been included in the deployment https://github.com/oodrive/leveldb.net

- this experiment uses the most recent ipfs kubo command line tool found here https://github.com/ipfs/kubo/releases/download/v0.22.0/kubo_v0.22.0_windows-amd64.zip

- see the example transaction JSON files https://github.com/embiimob/Sup/tree/master/P2FK/samples to begin experimenting with the entry and trading of your own Sup!? objects

- Sup!? blockchain objects can be discovered and browsed without internet access because Sup!? is communicating with a local copy of the blockchain

## **explore Sup!? on youtube**
 https://www.youtube.com/playlist?list=PLDNMoJ2rHmfoxt1AX417-lWt2zvWUnKUH
