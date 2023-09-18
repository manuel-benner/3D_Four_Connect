## Configure Unity For Version Control
### With your project open in the Unity editor:

- Open the editor settings window.
  - Edit > Project Settings > Editor
- Make .meta files visible to avoid broken object references.
  - Version Control / Mode: “Visible Meta Files” (if you can't find a "Version Control" tab, just serach for it in the global settings searchbar)
- Use plain text serialization to avoid unresolvable merge conflicts.
  - Asset Serialization / Mode: “Force Text”
- Save your changes.
  - File > Save Project
### This will affect the following lines in your editor settings file:

- ProjectSettings/EditorSettings.asset
  - m_ExternalVersionControlSupport: Visible Meta Files
  - m_SerializationMode: 2
- If you’re curious, you can read more about Unity’s YAML scene format [here](https://docs.unity3d.com/Manual/FormatDescription.html).