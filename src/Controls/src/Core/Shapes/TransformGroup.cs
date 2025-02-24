using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Maui.Controls.Shapes
{
	/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/TransformGroup.xml" path="Type[@FullName='Microsoft.Maui.Controls.Shapes.TransformGroup']/Docs/*" />
	[ContentProperty("Children")]
	public sealed class TransformGroup : Transform
	{
		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/TransformGroup.xml" path="//Member[@MemberName='ChildrenProperty']/Docs/*" />
		public static readonly BindableProperty ChildrenProperty =
			BindableProperty.Create(nameof(Children), typeof(TransformCollection), typeof(TransformGroup), null,
				propertyChanged: OnTransformGroupChanged);

		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/TransformGroup.xml" path="//Member[@MemberName='.ctor']/Docs/*" />
		public TransformGroup()
		{
			Children = new TransformCollection();
		}

		/// <include file="../../../docs/Microsoft.Maui.Controls.Shapes/TransformGroup.xml" path="//Member[@MemberName='Children']/Docs/*" />
		public TransformCollection Children
		{
			set { SetValue(ChildrenProperty, value); }
			get { return (TransformCollection)GetValue(ChildrenProperty); }
		}

		static void OnTransformGroupChanged(BindableObject bindable, object oldValue, object newValue)
		{
			if (oldValue != null)
			{
				(oldValue as TransformCollection).CollectionChanged -= (bindable as TransformGroup).OnChildrenCollectionChanged;
			}

			if (newValue != null)
			{
				(newValue as TransformCollection).CollectionChanged += (bindable as TransformGroup).OnChildrenCollectionChanged;
			}

			(bindable as TransformGroup).UpdateTransformMatrix();
		}

		void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (args.NewItems != null)
				foreach (INotifyPropertyChanged item in args.NewItems)
				{
					item.PropertyChanged += OnTransformPropertyChanged;
				}

			if (args.OldItems != null)
				foreach (INotifyPropertyChanged item in args.OldItems)
				{
					item.PropertyChanged -= OnTransformPropertyChanged;
				}

			UpdateTransformMatrix();
		}

		void OnTransformPropertyChanged(object sender, PropertyChangedEventArgs args)
		{
			UpdateTransformMatrix();
		}

		void UpdateTransformMatrix()
		{
			var matrix = new Matrix();

			foreach (Transform child in Children)
				matrix = Matrix.Multiply(matrix, child.Value);

			Value = matrix;
		}
	}
}