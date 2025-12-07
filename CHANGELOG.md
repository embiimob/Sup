# Changelog

## Repository Cleanup - December 2024

### Removed
- **Vendored Dependencies (includes/ directory)**: Removed 56 DLL files and 23 XML documentation files that are now managed via NuGet PackageReference. Total space saved: ~91MB
  - AngleSharp.dll and AngleSharp.Css.dll
  - NAudio family (NAudio.dll, NAudio.Asio.dll, NAudio.Core.dll, NAudio.Midi.dll, NAudio.Wasapi.dll, NAudio.WinForms.dll, NAudio.WinMM.dll)
  - Microsoft.Web.WebView2 assemblies (Core, WinForms, Wpf)
  - NBitcoin.dll
  - Newtonsoft.Json.dll
  - NReco.VideoConverter.dll
  - CommandLine.dll
  - HtmlSanitizer.dll
  - Gma.QrCodeNet.Encoding.dll
  - Various System.* assemblies that are available via NuGet
  - All associated XML documentation files

- **Legacy Package Management**: Removed packages.config file in favor of modern PackageReference format

### Updated
- **ipfs.exe**: Updated from Kubo v0.22.0 to v0.39.0 (latest release as of December 2024)
- **SUP.csproj**: Migrated from packages.config to PackageReference format for cleaner, more maintainable dependency management
- **.gitignore**: Added entries to exclude NuGet packages directory

### Kept (Retained for Application Functionality)
- **Cryptocurrency Node Executables**: bitcoin-qt.exe, dogecoin-qt.exe, litecoin-qt.exe, maza-qt.exe (~127MB total)
  - These are required runtime dependencies for blockchain interaction
- **IPFS Executable**: ipfs/ipfs.exe (now v0.39.0, 84MB)
  - Required for IPFS daemon functionality
- **Resource Files in includes/**: All non-DLL files (images, GIFs, HTML templates, etc.) remain in the includes/ directory as they are application resources

### Migration Notes
All removed DLL dependencies are now automatically restored via NuGet when running:
```
dotnet restore
```

The following packages are now managed via PackageReference:
- AngleSharp (0.17.1)
- AngleSharp.Css (0.17.0)
- NAudio family (2.1.0)
- Microsoft.Web.WebView2 (1.0.2045.28)
- NBitcoin (7.0.23)
- Newtonsoft.Json (13.0.2)
- NReco.VideoConverter (1.2.1)
- All Microsoft.Extensions.* packages (8.0.x)
- All Microsoft.AspNetCore.* packages (2.3.0)
- And 102 packages total

### Why These Changes?
1. **Reduced Repository Size**: Removed ~91MB of vendored binary dependencies
2. **Easier Maintenance**: NuGet PackageReference provides automatic dependency resolution and version management
3. **Better Security**: Easier to identify and update packages with security vulnerabilities
4. **Modern Standards**: PackageReference is the current standard for .NET projects
5. **Updated Dependencies**: Upgraded IPFS to latest stable version (v0.39.0)

### Next Steps for Maintainers
1. Run `dotnet restore` to download all NuGet packages
2. Run `dotnet build` to verify the project builds successfully
3. Test application functionality, especially:
   - IPFS integration (now using v0.39.0)
   - Cryptocurrency node interactions
   - WebView2 controls
   - Audio playback (NAudio)
   - Video conversion (NReco)
4. Verify all existing features work as expected

### Restoring Removed Files
If any of the removed vendored DLLs are needed for historical reference, they can be retrieved from git history:
```bash
git checkout <previous-commit> -- includes/
```

However, the recommended approach is to use the NuGet packages as they are kept up-to-date and include proper versioning.
