# Sup!? 
experimental decentralized state engine browser

Sup!? is a demonstration of establishing a decentralized identity and tracking state changes with comments on immutable blockchain objects in a format familiar to NFT enthusiasts. The experimental Sup!? object browser is currently hardcoded to reference the bitcoin testnet mainchain. A production deployment will be released December 2023. Our goal is to give #teamworld ample time for testing and integrations.

This experiment is using an ancient blockchain protocol called P2FK (Pay to Future Key) invented by http://HugPuddle.org in 2013.

P2FK was directly inspired by the satoshi uploader  see --> https://cirosantilli.com/satoshi-uploader

 As of Sup?'s first release date, all transactions and objects discoverable in the experiment were created by embii using http://apertus.io

## **Installation**

1. Download the Sup.v0.1.32-beta.zip file
2. Create a folder on your **fastest** disk drive with at least **80GB** free (it will be storing the bitcoin testnet blockchain)
3. Unzip all contents into the folder
4. Create a shortcut to SUP.exe and launch

**You will need to sync a blockchain to perform your first Sup!? object search**
1. Click the key button
2. Launch a full bitcoin testnet mainchain node using the button provided
3. Wait until testnet is fully synced. ( this part will take several hours )
4. Type #flowersofwar in the main search box and hit enter
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
searches mainchain for up to a 20 character case sensitive keyword and shows all object associations

### **@UserName**
searches mainchain by urn - shows currently registered profile object associated with @username and shows all object associations. profile registrations are transferable and expire after 3 years of no activity allowing them to be claimed by other registrants in the future.

### **ipfs://QmU42aLJToYmshwJu26iprH9RsX6SfJDw3FTf6senJEdF1**
performs a ipfs get, archives ipfs file(s) locally, pins file(s) if pinning is enabled, and opens a system folder dioluge allowing you to explore any files found

### **sup://twitter.com**
searches mainchain by urn - shows currently registered object with uri redirection.  urn registrations are transferable and expire after 3 years of inactivity allowing them to be claimed by other registrants in the future.  ( there are no urn character limits, uri redirects are NOT enabled )

### **http(s)://embii.org**
navigates to requested url in browser.
<br />
<br />
## **Profile Minting / Editing**
Click the mint button and select the profile mint button. Enter a urn and a profile address or click the diamond button to generate a new profile address. These two fields are all that are required to register a profile urn. You may optionally enter a PFP image url, your full name and suffix, web links, contact information and keywords. NOTE: Every profile becomes an entry point for which private messages may be delivered as every profile displays public keys to be used for Secp256k1 encrypted messaging.   

Once you are satisfied with your profile information. click mint. Your profile will take effect following a successfull confirmation of the transaction.

To edit a profile, click on the magnifying glass and enter the profile address to edit. If a profile is found at the address, Sup!? will build and display it on the screen. Modifying any of the fields will cause the corresponding fields' modified indicator to turn blue. Remove any data that is not being updated from the transaction by clicking its corresponding button until it is not blue. Click mint when you have completed making updates. Your changes will take effect following a successful confirmation of the transaction.

Your profile urn registration is valid for 3 years past the last change in its process height. This means that in order for your profile to expire, there must not be a single mention or transaction placed to its holding address for a period of 3 years. Should this occur, the urn registration will be claimed by the next available registration that falls under the same time restriction. If no claims exist, the profile will no longer be discoverable and can be claimed by anyone. An inactive profile can be reactivated by anyone by sending any type of transaction to its previous holding address.
<br />
<br />
## **Object Minting / Printing / Editing**
Click the mint button and select the object mint button. Enter an object name, object address and urn. These fields are required. Click mint. If no creator address is entered, the object will be self-signed using a new address generated at your minting station wallet. To further enhance your collector's experience, a profile can be setup on a collection address and the collection addresss can be added as a creator. Now, both the collection shortname name and the creator short name will be discoverable in the browser. Searching for the creator shortname returns all objects.  Searching for the collection shortname returns just the collection.

Click print instead of mint and you can use a paper transaction alone to prove ownership. Any person can scan a paper mint & send a small confirmation transaction to the addresses within the QR code printed on the paper to make the paper transaction public. A Sup!? paper transaction can be used in lieu of a public confirmation as it contains all the necessary signature and transaction data necessary to confirm it against a current scan of the object address it is associated with.

To edit an unlocked object, click on the magnifying glass and enter the object address to edit or scan a paper mint. If an address is found and/or the paper transaction is a valid transaction, Sup!? will build and display the object it represents on screen. Modifying any of the fields will cause the corresponding fields' modified indicator to turn blue.. Click mint Or print when you have completed making updates and your changes will take effect following a successfull confirmation of the transaction.
<br />
<br />
## **Live Monitor**
Click the live button and watch in realtime as other Sup!? users mint their objects. All new objects will be added to the top of your browser window pushing the older objects down the screen. A maximized Sup!? monitor that shows a constant stream of new objects makes for a great display. Sup!? montiors the in-memory pool allowing it to display new objects before they are confirmed in the blockchain. Your Sup!? objects will appear in every curently live Sup!? montior around the world in about 10 - 30 seconds after you have etched the object's metadata. All blockhains included with this experiment are monitored for new objects if they are running and fully synced.  
<br />
With live monitoring enabled, you never know what might pop into the window. The damage is limited to a thumbnail image and what can fit in a small description text box. Objects that are discovered in live mode are not fully inspected for safety reasons. You can delete one directly from the live preview panel by clicking on the object's small trash icon. This will remove any trace of it from your system. If you are still concerned, clearing all Sup!? caches will without a doubt remove it.
<br />
<br />
## **Official / Unofficial Registration Indicator**
When a Sup!? urn is registered, a small symbol appears on the top left corner of the object's thumbnail on the main browser screen, replacing the urn label on the object details screen. Following registration, any other Sup!? objects referencing the same urn will display a [ SEE OFFICIAL ] button that redirects them to the officially registered urn. The unofficial urn reference can still be opened by clicking anywhere elese on the object.  
<br />
<br />
## **File Lookup**
Drag and drop any file onto the main Sup!? browser screen. Sup!? will search the mainchain for a current file registration with an owner. If found, the registered object will be returned in the main browser window. To obtain a file registration keyword necessary to register a file, drag and drop the file onto the workbench, the reference key will be displayed in the workbench output window. See the example transaction JSON files https://github.com/embiimob/Sup/tree/master/P2FK/samples to begin experimenting with Sup!? file lookups and urn registration.
<br />
<br />
## **Block \ Mute**
Blocking addresses ignores any associations to them going forward, preventing a transaction or any transactions signed by the blocked address from outputting to disk. A purge and reinspection is required to completely eliminate all data. If a group of collectors decides to block an address, they can technically ignore them out of existence.

Muting an address prevents any further transaction comments signed by the address from outputting onto a disk. A purge and reinspection is required to completely elimninate all transaction comments.
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

- this experiment uses the most recent ipfs kubo command line tool found here https://dist.ipfs.tech/kubo/v0.18.1/kubo_v0.18.1_windows-amd64.zip

- see the example transaction JSON files https://github.com/embiimob/Sup/tree/master/P2FK/samples to begin experimenting with the entry and trading of your own Sup!? objects

- Sup!? blockchain objects can be discovered and browsed without internet access because Sup!? is communicating with a local copy of the blockchain

## **explore Sup!? on youtube**
 https://www.youtube.com/playlist?list=PLDNMoJ2rHmfoxt1AX417-lWt2zvWUnKUH
