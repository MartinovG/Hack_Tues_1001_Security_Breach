using HackTues.Engine;

namespace HackTues.Editor;

public class ModifiableHitbox {
    public Hitbox Value { get; set; }

    public ModifiableHitbox(Hitbox value) {
        Value = value;
    }
}
