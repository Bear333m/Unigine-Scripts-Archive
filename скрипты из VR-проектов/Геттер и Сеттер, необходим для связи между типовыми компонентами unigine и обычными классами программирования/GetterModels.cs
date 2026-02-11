using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "8167cee0eeb23bffc9cf833a5891f78064051072")]
public class GetterModels : Component
{
    [ShowInEditor] private ObjectGui storedGui;
    [ShowInEditor] private Node nodee;
    [ShowInEditor] private Node vrPlayer;

    private TreeOfModel treeOfModel;
    private CameraCast cameraCast;

    public void InitializeGui(ObjectGui gui)
    {
        storedGui = gui;
    }

    // Геттер для внешнего доступа
    public ObjectGui GetGui()
    {
        return storedGui;
    }

    public Node GetNode()
    {
        return nodee;
    }

    public int GetNodeID()
    {
        return nodee.ID;
    }

    public Node GetPlayer()
    {
        return vrPlayer;
    }

    void Init()
    {
        cameraCast = new CameraCast(this);
        treeOfModel = new TreeOfModel(this, storedGui);
        treeOfModel.CreateTree(nodee.ID);
    }

}

