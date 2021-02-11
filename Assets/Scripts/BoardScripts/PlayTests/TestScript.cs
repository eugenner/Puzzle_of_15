using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Tests
{
    public class TestScript
    {
        private InitBoard initBoard;

        [SetUp]
        public void Setup()
        {
            SceneManager.LoadScene("Scenes/SampleScene", LoadSceneMode.Single);
            //var go = GameObject.Instantiate(new GameObject());
            //initBoard = go.AddComponent<InitBoard>();
            

        }

        [UnityTest]
        public IEnumerator TestScriptSimplePasses2()
        {
            yield return null;
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
            Scene activeScene = SceneManager.GetActiveScene();
            Assert.IsTrue(activeScene.isLoaded);
            var rootGameObjects = activeScene.GetRootGameObjects();
            Assert.IsNotNull(rootGameObjects, "Missing rootGameObjects ");
            GameObject go = GameObject.Find("Board");
            Assert.IsNotNull(go, "Missing go ");


        }

        [UnityTest]
        public IEnumerator TestCheckGameEndPosition()
        {
            yield return null;
            initBoard = GameObject.Find("Board").GetComponent<InitBoard>();
            Assert.AreEqual(initBoard.tilePositions[3, 3].tileNumber, 16, "The last tile is not 16");
            Assert.IsTrue(initBoard.CheckGameEndPosition());
        }

        [UnityTest]
        public IEnumerator TestInversionCount()
        {
            yield return null;
            initBoard = GameObject.Find("Board").GetComponent<InitBoard>();
            Assert.AreEqual(initBoard.InversionCount(new ArrayList(new int[] { 6, 3, 2, 4, 7, 5, 1, 0 })), 19);

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
            initBoard = GameObject.Find("Board").GetComponent<InitBoard>();
            //initBoard.Init();
            foreach (var tp in initBoard.tilePositions)
            {
                Debug.Log("row: " + tp.row + " col: " + tp.col + " pos: " + tp.tilePivot.ToString());
            }

            var activeScene = SceneManager.GetActiveScene();
            Assert.IsTrue(activeScene.isLoaded);
            var rootGameObjects = activeScene.GetRootGameObjects();

            Assert.IsNotNull(rootGameObjects, "Missing rootGameObjects ");
            Assert.Greater(initBoard.tilePivot.Length, 0, "tilePivot[] is initialized");
            

            initBoard.ShuffleTiles();
            Assert.IsTrue(true);
            yield return new WaitForSeconds(10);
        }
    }
}