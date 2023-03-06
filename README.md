# Sup!? 
experimental decentralized state engine browser

Sup!? is a READ ONLY demonstration of establishing a decentralized identity and tracking state changes with comments on immutable blockchain objects in a format familiar to NFT enthusiasts. the experimental Sup!? object browser is currently hardcoded to reference the bitcoin testnet mainchain ONLY. a production read / write deployment will be released December 2023, our goal is to give #teamworld ample time for testing and integrations

This experiment is using an ancient blockchain protocol called P2FK (Pay to Future Key) invented by http://HugPuddle.org in 2013.

P2FK was directly inspired by the satoshi uploader  see --> https://cirosantilli.com/satoshi-uploader

 As of Sup?'s first release date all transactions and objects discoverable in the experiment were created by embii using http://apertus.io

## **Installation**

1. Download the Sup.v0.1.26-beta.zip file
2. Create a folder on your **fastest** disk drive with at least **700GB** free (it will be storing the bitcoin prod and testnet blockchains)
3. Unzip all contents into the folder.
4. Create a shortcut to SUP.exe and launch.

**You will need to sync two blockchains to perform you first Sup!? object search**
1. Click the key button
2. Launch a full bitcoin Production node AND a full Bitcoin testnet node using the buttons provided
3. Wait until testnet is fully synched and production has synced passed at least 2014. ( this part could take several hours )
4. Type #flowersofwar in the main search box and hit enter
<br />
<br /> 

## **Search Options**

### **muVrFVk3ErfrnmWosLF4WixxRtDKfMx9bs**
searches mainchain - bitcoin testnet by address returns all object associations and or associated profile

### **6d14b0dc526a431f611f16f29d684f73e6b01f0a59a0b7b3d9b8d951091c2422/index.html**
searches mainchain - bitcoin testnet by transaction id returns referenced index.htm(l) in browser

### **BTC:0618f12af65a4e82f8e7b41f8578721dfeb109e9a73ff71aebdbc982696e3720/index.html**
searches sidechain - bitcoin production by transaction id returns referenced index.htm(l) in browser

### **MZC:4dbb0e984586d1994f461c419c460edf7ecf15488a8b11282f19cec9aa7ec285/index.html?viewer=embii4u**
searches sidechain - mazacoin production by transaction id returns referenced index.htm(l) in browser

### **#CaseSensitiveKeyword**
searches mainchain for up to a 20 character case sensitive keyword and shows all object associations

### **@UserName**
searches mainchain by urn shows currently registered profile object associated with @userName and shows all object associations.  profile registrations are transferable and expire after 3 years of no activity allowing them to be claimed by other registrants in the future

### **ipfs://QmU42aLJToYmshwJu26iprH9RsX6SfJDw3FTf6senJEdF1**
performs a ipfs get, archives ipfs file(s) locally, pins file(s) if pinning is enabled, opens a system folder dioluge allowing you to explore any files found

### **sup://twitter.com**
searches mainchain by  urn shows currently registered object with uri redirection.  urn registrations are transferable and expire after 3 years of no activity allowing them to be claimed by other registrants in the future.  ( there are no urn character limits, uri redirects are NOT enabled )

### **http(s)://embii.org**
navigates to requested url in browser.
<br />
<br />
## **Live Monitor**
Click the live button and watch in realtime as other Sup!? user's mint their objects.  all new objects will be added to the top of your browser window pushing the older objects down the screen. A maximized Sup!? monitor that shows a constant stream of new objects makes for a great display.  Sup!? montiors the in memory pool allowing it to display new objects before they are confirmed in the blockchain.  your Sup!? objects will appear in every curently live Sup!? montior around the world in about 10 - 30 seconds after you have etch the object's metadata. all blockhains included with this experiment are monitored for new objects if they are running and fully synced 
<br />
<br />
## **Official / Unofficial Registration Indicator**
When a Sup!? urn is registered a small symbol appears on the top left corner of the object's thumbnail on the main browser screen and replaces the URN label on the object details screen. following registration, any other Sup!? objects referencing the same URN will display a [ SEE OFFICIAL ] button that redirects them to the officialy registered urn.  the unofficial urn reference can still be opened by clicking anywhere elese on the object.  
<br />
<br />
## **File Lookup**
Drag and drop any file onto the main Sup!? browser screen.  Sup!? will search the mainchain for a current file registration with owner.  if found the registered object will be returned in the main browser window. to obtain a file registration keyword necessary to register a file, drag and drop the file onto the workbench, the reference key will be displayed in the workbench output window.  see the example transaction JSON files https://github.com/embiimob/Sup/tree/master/P2FK/samples to begin experimenting with Sup!? file lookups and urn registration
<br />
<br />
## **Block \ Mute**
blocking addresses ignores any associations to them going forward preventing a transaction or any transactions signed by the blocked address from outputting to disk. a purge and reinspection is required to completely eliminate all data. if a group of collectors decides to block an address they can technically ignore them out of existence.

muting an address prevents any further transaction comments signed by the address from outputting onto disk.  a purge and reinspection is required to completely elimninate all transactions comments
<br />
<br />
## **Web Applet Querystring Data**
trusted index.html and index.htm files are sent the following Sup!? object information via querystring on load
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
http://bitfossil.org/7033eb8138de0d3f4be111a57dfb8319b400d3b6a6f5b387a22b334ebb998e93/EMBII.jpg<br /> 
( references a http(s) address )<br />
<br />


- this experiment uses a v0.13.2 full bitcoin core clone with address index change https://github.com/btcdrak/bitcoin/tree/addrindex-0.14

- this experiment uses litecoin core the following release is included in the deployment https://download.litecoin.org/litecoin-0.16.3/win/

- this experiment uses dogecoin core the following release is included in the deployment https://github.com/dogecoin/dogecoin/releases/tag/v1.14.6

- this experiment uses mazacoin core their latest official release has been included in the deployment https://github.com/mazacoin/maza

- this experiment uses oodrive's levelDB a stable release has been included in the deployment https://github.com/oodrive/leveldb.net

- this experiment uses the most recent ipfs kubo command line tool found here https://dist.ipfs.tech/kubo/v0.18.1/kubo_v0.18.1_windows-amd64.zip

- see the example transaction JSON files https://github.com/embiimob/Sup/tree/master/P2FK/samples to begin experimenting with the entry and trading of your own Sup!? objects

- Sup!? blockchain objects can be discovered and browsed without internet access because Sup!? is communicating with a local copy of the blockchain.

## **explore Sup!? on youtube**
 https://www.youtube.com/playlist?list=PLDNMoJ2rHmfoxt1AX417-lWt2zvWUnKUH
