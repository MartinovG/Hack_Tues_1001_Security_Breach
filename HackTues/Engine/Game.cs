using HackTues.Controls;

namespace HackTues.Engine;

public class Game {
    public IPlayer? Player { get; set; }
    public Map Map { get; set; } = new();

    public void Render(float delta) {
        var set = new SortedSet<Layer>();
        Map.AddLayers(set);
        Player?.AddLayers(set);

        foreach (var fg in set) {
            fg.Render();
        }
    }
    public void Update(IController? controller, float delta) {
        Player?.Update(delta, Map, controller);
    }
}