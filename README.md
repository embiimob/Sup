# Sup!? 
experimental decentralized state engine browser

Sup!? Object Browser is currently a READ ONLY demonstration of establishing a decentralized identity and tracking state changes with comments on immutable blockhain objects in a format familiar to NFT enthusiasts. 

This experiment is using an ancient blockchain protocol called P2FK (Pay to Future Key) invented by http://HugPuddle.org in 2013 .

P2FK was directly inspired by the satoshi uploader  see --> https://cirosantilli.com/satoshi-uploader

 As of Sup?'s first release date all transactions and objects discoverable in the experiment were created by embii using http://apertus.io

### **Installation**

1. Download the Sup.v0.1.4-beta.zip file
2. Create a folder on your **fastest** disk drive with at least **700GB** free (it will be storing the bitcoin prod and testnet blockchains)
3. Unzip all contents into the folder.
4. Create a shortcut to SUP.exe and launch.

**You will need to sync two blockchains to perform you first Sup!? object search**

1. Click the key button
2. Launch a full bitcoin Production node AND a full Bitcoin testnet node using the buttons provided
3. Wait until testnet is fully synched and production has synced passed at least 2014. ( this part could take several hours )
4. Type #flowersofwar in the main search box and hit enter


### **Search Options**

### **#CaseSensitiveKeyword**

searches up to a 20 character case sensitive keyword and shows all object associations
 
### **sup://twitter.com**

searches urn shows currently registered object with URI  ( no urn character limits, redirects are NOT enabled )

### **@UserName**

searches urn shows currently registered profile object associated with @userName and shows all object associatins

### **ipfs://QmU42aLJToYmshwJu26iprH9RsX6SfJDw3FTf6senJEdF1**

performs a ipfs get, archives ipfs file(s) locally, pins file(s) if pinning is enabled, opens a system folder dioluge allowing you to explore any files found

### **Block \ Mute**

- blocking addresses and transactions ignores any associations to them going forward preventing a transaction or any transactions signed by the blocked address from outputting to disk. a purge and reinspection is required to completely eliminate all data. if a group of collectors decides to block an address or transaction they can technically ignore them out of existence.

- muting an address prevents any further transaction comments signed by the address from outputting onto disk.  a purge and reinspection is required to completely elimninate all transactions comments

### **Web Applet Querystring Data**

trusted index.html and index.htm files are sent the following Sup!? object information via querystring on load
"address=", "creator=", "viewer=", "viewer-name=", "owner=", "owner-name=", "urn=", "uri=", "url=", "height="

### **URN IMG & URI Referencing Standards**

-  IPFS:Qma7o6Yx2HQfCkNJEHv1gpiDzoZU8rNc6JFQXijfyt8cGc

( references an IPFS gif file creates a default file named {hash}\artifact )

-  IPFS:Qmex6VRDqwVCMPrc7ojtBWzUBPdQAmWq9FcprKTcgmbKX3/baby punch.glb

( references and IPFS file output file with given name {hash}\baby punch.glb )

-  66e5f4df4bd0a3ea9b569498ed25f848e837b9dec7a2699c1d6756ae9560c962/8354.png

( references mainchain - bitcoin testnet P2FK)

-  MZC:c0c7fa9536e31e04a65252d8acec29af1c54cb0a44609af7fb9e3804065c7f19/chief-sitting-bull.jpg

( references sidechain - mazacoin production P2FK )

-  BTC:3ff52882c93420c8fe4a90f6fa94b2a563316b5e7b83afe2ddd5bcadc86d3821/FakeUFO.png

( references sidechain - bitcoin production P2FK )

-  http://bitfossil.org/7033eb8138de0d3f4be111a57dfb8319b400d3b6a6f5b387a22b334ebb998e93/EMBII.jpg 

( references a http(s) address )

**ASCII free text urn registrations such as 'twitter.com' cannot begin with 'IPFS:',{transaction id},'MZC:','BTC:' or 'HTTP'**


### 
- this experiment uses a v0.13.2 full bitcoin core clone with address index change https://github.com/btcdrak/bitcoin/tree/addrindex-0.14

- this experiment uses mazacoin their latest official release has been included in the deployment https://github.com/mazacoin/maza

- this experiment uses oodrive's levelDB a stable release has been included in the deployment https://github.com/oodrive/leveldb.net

- this experiment uses the most recent ipfs kubo command line tool found here https://dist.ipfs.tech/kubo/v0.18.1/kubo_v0.18.1_windows-amd64.zip

- see the example transaction JSON files https://github.com/embiimob/Sup/tree/master/P2FK/samples to begin experimenting with the entry and trading of your own Sup!? objects

- Sup!? blockchain objects can be discovered and browsed without internet access because Sup!? is communicating with a local copy of the blockchain.

### **explore Sup!? on youtube**
 https://www.youtube.com/playlist?list=PLDNMoJ2rHmfoxt1AX417-lWt2zvWUnKUH
 



