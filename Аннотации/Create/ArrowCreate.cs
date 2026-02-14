using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;

namespace UnigineApp.data.Code.Annotation.Arrow
{
    internal class ArrowCreate
    {
        public void Arrow(vec3 start, vec3 end, float size, int layers, int type, vec4 color, bool numBool)
        {
            Mesh mesh = new Mesh();
            mesh.AddBoxSurface("mesh1", new vec3(size * 0.01f));
            var dynamicMesh = new ObjectMeshDynamic(mesh);
            dynamicMesh.SetIntersection(true, 0);
            dynamicMesh.SetMaxVisibleDistance(0.0000000001f, 0);
            dynamicMesh.WorldTransform = MathLib.Translate(start);
            dynamicMesh.Name = "ArrowMaingT5DxmxyMif9tnZkewPlIV";

            Mesh mesh2 = new Mesh();
            mesh2.AddBoxSurface("mesh2", new vec3(size * 0.01f));
            var dynamicMesh2 = new ObjectMeshDynamic(mesh);
            dynamicMesh2.SetIntersection(true, 0);

            dynamicMesh2.SetMaxVisibleDistance(0.0000000001f, 0);

            Storage.ArrowID.Add(dynamicMesh.ID);
            Storage.ArrowType.Add(type);
            Storage.ArrowSize.Add(size);
            Storage.ArrowWidth.Add(layers);
            Storage.ArrowColor.Add(color);
            Storage.ArrowNumbers.Add(numBool);

            //dynamicMesh2.Name = "Arrow_" + dynamicMesh.ID.ToString();
            dynamicMesh2.Name = "ArrowgT5DxmxyMif9tnZkewPlIV";
            dynamicMesh.AddChild(dynamicMesh2);

            dynamicMesh2.WorldTransform = MathLib.Translate(end);

        }
    }
}
