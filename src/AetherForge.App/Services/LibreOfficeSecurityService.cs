using System.IO;
using System.Text;

namespace AetherForge.App.Services;

internal static class LibreOfficeSecurityService
{
    public static async Task PrepareProfileAsync(
        string profileDirectory,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string userDirectory = Path.Combine(profileDirectory, "user");
        Directory.CreateDirectory(userDirectory);

        // Each conversion starts with its own profile, before LibreOffice loads it.
        // These settings prevent execution; they do not remove macros from outputs.
        const string configuration = """
            <?xml version="1.0" encoding="UTF-8"?>
            <oor:items xmlns:oor="http://openoffice.org/2001/registry">
              <item oor:path="/org.openoffice.Office.Common/Security/Scripting">
                <prop oor:name="DisableMacrosExecution" oor:op="fuse"><value>true</value></prop>
                <prop oor:name="MacroSecurityLevel" oor:op="fuse"><value>3</value></prop>
                <prop oor:name="DisableActiveContent" oor:op="fuse"><value>true</value></prop>
                <prop oor:name="BlockUntrustedRefererLinks" oor:op="fuse"><value>true</value></prop>
              </item>
              <item oor:path="/org.openoffice.Office.Writer/Content/Update">
                <prop oor:name="Link" oor:op="fuse"><value>0</value></prop>
              </item>
              <item oor:path="/org.openoffice.Office.Calc/Content/Update">
                <prop oor:name="Link" oor:op="fuse"><value>1</value></prop>
              </item>
            </oor:items>
            """;

        await File.WriteAllTextAsync(
            Path.Combine(userDirectory, "registrymodifications.xcu"),
            configuration,
            new UTF8Encoding(false),
            cancellationToken);
    }
}
