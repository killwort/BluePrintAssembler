using System;
using System.Drawing;
using System.Runtime.Serialization;
using BluePrintAssembler.Domain;
using BluePrintAssembler.Utils;

namespace BluePrintAssembler.UI.VM
{
    [Serializable]
    public class ProducingEntity:ISerializable
    {
        private NotifyTaskCompletion<Bitmap> _icon;
        private BaseProducingEntity _myEntity;
        public ProducingEntity(){}
        public ProducingEntity(BaseProducingEntity entity)
        {
            this.MyEntity = entity;
        }

        public BaseProducingEntity MyEntity
        {
            get { return _myEntity; }
            set
            {
                _myEntity = value;
                _icon = null;
            }
        }

        public NotifyTaskCompletion<Bitmap> Icon => _icon ?? (_icon = new NotifyTaskCompletion<Bitmap>(Configuration.Instance.GetIcon(MyEntity)));
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Type",_myEntity.Type);
            info.AddValue("Name",_myEntity.Name);
        }
        public ProducingEntity(SerializationInfo info, StreamingContext context)
        {
            MyEntity = (BaseProducingEntity) Configuration.Instance.RawData.GetEntity(info.GetString("Type"), info.GetString("Name"));
        }
    }
}