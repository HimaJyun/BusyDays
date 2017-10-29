using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BusyDays.Common.Tests {
    [TestClass()]
    public class TsvSerializerTests {

        [TestMethod()]
        public void TsvSerializerTest() {
            new TsvSerializer<TestType>("./tmp");
            new TsvSerializer<TestTypePrivate>("./tmp");
        }

        [TestMethod()]
        public void TsvSerializerTest1() {
            new TsvSerializer<TestType>("./tmp", Encoding.Unicode);
            new TsvSerializer<TestTypePrivate>("./tmp", Encoding.Unicode);
        }

        [TestMethod()]
        public void SerializeTest() {
            var path = Path.GetTempFileName();
            var serializer = new TsvSerializer<TestType>(path);
            var serializerPrivate = new TsvSerializer<TestTypePrivate>(path);

            TestType[] testType = {
                new TestType(),
                new TestType(),
            };
            TestTypePrivate[] testTypePrivate = {
                new TestTypePrivate(),
                new TestTypePrivate(),
            };

            d("Serialize:");
            serializer.Serialize(testType);
            d(File.ReadAllText(path));

            d("Serialize(private):");
            serializerPrivate.Serialize(testTypePrivate);
            d(File.ReadAllText(path));
            File.Delete(path);
        }

        [TestMethod()]
        public void DeserializeTest() {
            var path = Path.GetTempFileName();
            var serializer = new TsvSerializer<TestType>(path);

            TestType[] testType = {
                new TestType(),
                new TestType(),
            };

            d("Deserialize:");
            serializer.Serialize(testType);
            d(File.ReadAllText(path));
            var testType2 = serializer.Deserialize().ToArray();
            File.Delete(path);

            Assert.IsNotNull(testType2, "デシリアライズ出来てない");
            Assert.IsTrue(testType.Length == testType2.Length, "結果の要素数が違う");
            Assert.AreEqual(testType[0], testType2[0], "結果が違う");
            Assert.AreEqual(testType[1], testType2[1], "結果が違う");

            // ファイル無しの場合のテスト
            testType2 = serializer.Deserialize().ToArray();
            Assert.IsTrue(testType2.Length == 0, "ファイルがないのに中身が空じゃない");

            // 空ファイルのテスト
            path = Path.GetTempFileName();
            testType2 = serializer.Deserialize().ToArray();
            Assert.IsTrue(testType2.Length == 0, "ファイルが空なに中身が空じゃない");
            File.Delete(path);
        }

        [TestMethod()]
        public void AddTypeConverterTest() {
            var path = Path.GetTempFileName();
            var serializer =
                new TsvSerializer<TestType>(path).AddTypeConverter(typeof(string), new StringConverter());

            d("AddTypeConverterTest:");
            serializer.Serialize(new TestType[] { new TestType() });
            d(File.ReadAllText(path));

            File.Delete(path);
        }

        private void d(object s) {
            Debug.WriteLine(s);
        }

        private class TestTypePrivate {
            public int A = 0;
        }
    }
    public class TestType {
        // 数値
        public int Number = 123;
        // 文字列
        public string Str = "\\string\t\\t";
        // null値
        public string Null = null;
        // Nullable値
        public int? Nullable = null;
        // 名前指定
        [TsvColumn(Name = "NameAttribute")]
        public int Name = 0;
        // 無視されるべき
        [TsvColumn(Ignore = true)]
        public int Ignore = 0;
        // 浮動小数
        public double Double = 123.456F;
        // マルチバイト文字
        public string MultiByteString = "日本語！";
        // プロパティ
        public int Property { get; set; } = 0;
        // private(無視されるべき)
        private string IgnorePrivate = "Ignore";
        // getonly、setonlyのプロパティ(無視されるべき)
        public int GetOnlyProperty { get; } = 0;
        public int SetOnlyProperty { set { this.Ignore = value; } }

        public override bool Equals(object obj) {
            var o = obj as TestType;
            if (o == null) {
                return false;
            }

            // ハッシュ衝突とか知らねーし！
            return this.GetHashCode() == obj.GetHashCode();
        }
        public override int GetHashCode() {
            int i = 0;
            i ^= Number.GetHashCode();
            i ^= Str?.GetHashCode() ?? 0;
            i ^= Null?.GetHashCode() ?? 0;
            i ^= Nullable?.GetHashCode() ?? 0;
            i ^= Name.GetHashCode();
            i ^= Ignore.GetHashCode();
            i ^= Double.GetHashCode();
            i ^= MultiByteString?.GetHashCode() ?? 0;
            i ^= Property.GetHashCode();
            i ^= IgnorePrivate?.GetHashCode() ?? 0;
            i ^= GetOnlyProperty.GetHashCode();
            return i;
        }
    }
}