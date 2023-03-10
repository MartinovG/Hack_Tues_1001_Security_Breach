namespace HackTues.Engine; 

public interface ILayerOwner {
    void AddLayers(SortedSet<Layer> layers);
}
