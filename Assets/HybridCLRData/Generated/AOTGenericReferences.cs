public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ constraint implement type
	// }} 

	// {{ AOT generic type
	//GameBaseFramework.Base.DataValue`1<System.Int32>
	//System.Action`1<System.Object>
	//System.Action`1<System.Int32>
	//System.Collections.Generic.Dictionary`2<System.Object,System.Object>
	//System.Collections.Generic.Dictionary`2<System.Int32,System.Object>
	//System.Collections.Generic.Dictionary`2<GameMain.Modules.ERedPointModule,System.Object>
	//System.Collections.Generic.Dictionary`2/Enumerator<System.Object,System.Object>
	//System.Collections.Generic.KeyValuePair`2<System.Object,System.Object>
	//System.Collections.Generic.List`1<System.Object>
	//System.Collections.Generic.List`1/Enumerator<System.Object>
	//System.Func`2<System.Object,System.Object>
	//System.Nullable`1<System.Int32>
	// }}

	public void RefMethods()
	{
		// System.String Bright.Common.StringUtil::CollectionToString<System.Object>(System.Collections.Generic.IEnumerable`1<System.Object>)
		// System.Void GameBaseFramework.Patterns.CommandManager::AddCommand<System.Object>(System.Object)
		// System.Void GameBaseFramework.Patterns.CommandManager::BindCommandReceiver<System.Object>(GameBaseFramework.Patterns.CommandReceiver)
		// System.Object[] System.Array::Empty<System.Object>()
		// System.Object UnityEngine.Component::GetComponent<System.Object>()
		// System.Object UnityEngine.GameObject::GetComponent<System.Object>()
		// System.Object UnityEngine.Object::Instantiate<System.Object>(System.Object,UnityEngine.Transform)
	}
}