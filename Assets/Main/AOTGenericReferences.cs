public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//GameBaseFramework.Base.DataValue`1<System.Int32>
	//System.Action`1<System.Int32>
	//System.Collections.Generic.Dictionary`2<GameMain.Modules.ERedPointModule,System.Object>
	//System.Collections.Generic.List`1<System.Object>
	// }}

	public void RefMethods()
	{
		// System.Object UnityEngine.AssetBundle::LoadAsset<System.Object>(System.String)
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::AddComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object,UnityEngine.Transform)
		// System.Object UnityEngine.Resources::Load<System.Object>(System.String)
	}
}