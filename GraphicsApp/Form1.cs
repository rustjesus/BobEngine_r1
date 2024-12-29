using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GraphicsApp.GameScripts;

namespace GraphicsApp
{
    public partial class Form1 : Form
    {
        private bool directx9 = false;
        //private bool directx10 = false;
        private bool directx11 = true;




        public static bool usingDirectx9;
        public static bool usingDirectx11;
        private SharpDX.Direct3D9.Device device9;
        private SharpDX.Direct3D11.Device device;
        private SharpDX.DXGI.SwapChain swapChain;
        private DeviceContext context;
        private Shader shader11;
        private Shader solidCubeShader;
        private Renderer renderer;
        private RendererDX9 rendererDX9;
        private Camera camera;
        private CameraUI cameraUI;
        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private SharpDX.Direct3D11.Buffer indexBuffer;
        private SharpDX.Direct3D11.Buffer constantBuffer;
        private Matrix worldMatrix;
        private float rotationSpeed;

        private SharpDX.Direct3D11.Buffer lightBuffer;  // Light buffer
        private Light light;
        //private Shadow shadow;
        private List<Resolution> availableResolutions;
        private List<ResolutionDX9> availableResolutionsDX9;
        private Resolution currentResolution; 
        private ResolutionDX9 currentResolutionDX9;
        private CollisionManager collisionManager;
        private Projectile projectile;
        private LineRenderer lineRenderer;
        private EnemyCubes enemyCubes;
        public Vector3[] cubePos;
        private Matrix localRot;
        private GameInput gameInput;
        public Form1()
        {
            usingDirectx9 = directx9;
            usingDirectx11 = directx11;
            InitializeComponent();
            // Initialize collision manager
            collisionManager = new CollisionManager();
            InitializeResolutions();
            UIManager.projectilesShot = 0;
            UIManager.CreateUI(this);
            input = new Input();
            random = new Random();
            projectile = new Projectile();
            enemyCubes = new EnemyCubes();
            gameInput = new GameInput();
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;


        }
        private void InitializeResolutions()
        {
            if(directx11)
            {

                availableResolutions = new List<Resolution>
                {
                    new Resolution(800, 600),
                    new Resolution(1024, 768),
                    new Resolution(1280, 720),
                    new Resolution(1920, 1080)
                };
                currentResolution = availableResolutions[0]; // Default resolution
            }
            if(directx9)
            {
                availableResolutionsDX9 = new List<ResolutionDX9>
                {
                    new ResolutionDX9(800, 600),
                    new ResolutionDX9(1024, 768),
                    new ResolutionDX9(1280, 720),
                    new ResolutionDX9(1920, 1080)
                };
                currentResolutionDX9 = availableResolutionsDX9[0]; // Default resolution

            }

        }
        private UIForm uiForm;
        private UIResolution uiRes;
        private void ShowUIForm()
        {
            if (uiForm == null || uiForm.IsDisposed)
            {
                var uiResolutions = new List<UIResolution>();
                foreach (var res in availableResolutions)
                {
                    uiResolutions.Add(new UIResolution(res.Width, res.Height));
                }

                if (directx11)
                    uiRes = new UIResolution(currentResolution.Width, currentResolution.Height);
                if(directx9)
                    uiRes = new UIResolution(currentResolutionDX9.Width, currentResolutionDX9.Height);


                uiForm = new UIForm(uiResolutions, uiRes);
                uiForm.ResolutionChanged += OnUIResolutionChanged;
                uiForm.RenderingApiChanged += OnRenderingApiChanged; // Attach event handler
                uiForm.Show(this);
            }
            else
            {
                uiForm.Focus();
            }

        }

        private void OnRenderingApiChanged(string selectedApi)
        {
            Console.WriteLine($"Rendering API changed to: {selectedApi}");

            if (selectedApi == "DirectX 9")
            {
                // Switch to DirectX 9 rendering
                InitializeGraphicsDirectX9();
            }
            else if (selectedApi == "DirectX 11")
            {
                // Switch back to DirectX 11 rendering
                InitializeGraphicsDirectX11();
            }
            //TODO ADD OPEN GL RENDERING USING SharpGL OR OpenTK
        }
        private void InitializeGraphicsDirectX9()
        {
            // Placeholder for DirectX 9 initialization logic
            Console.WriteLine("Switched to DirectX 9 rendering. Initialize DX9 graphics pipeline here.");
            // Implement DirectX 9 rendering initialization

            directx9 = true;
            directx11 = false;
            //TODO NEED TO FINISH: ADD DIRECT X
            InitializeGraphics(); // Reinitialize DirectX 11 pipeline
        }

        private void InitializeGraphicsDirectX11()
        {
            // Current DirectX 11 initialization logic here
            Console.WriteLine("Switched to DirectX 11 rendering. Reinitializing DX11 pipeline.");


            directx9 = false;
            directx11 = true;
            InitializeGraphics(); // Reinitialize DirectX 11 pipeline
        }
        private void OnUIResolutionChanged(UIResolution newResolution)
        {
            // Convert UIResolution back to Resolution if needed
            var convertedResolution = new Resolution(newResolution.Width, newResolution.Height);

            // Handle the resolution change here
            Console.WriteLine($"Resolution changed to: {convertedResolution.Width}x{convertedResolution.Height}");

            // Update the current resolution
            currentResolution = convertedResolution;
            OnResolutionChanged(currentResolution);
        }

        private void OnResolutionChanged(Resolution newResolution)
        {
            currentResolution = newResolution;
            currentResolution.ApplyRes(this, renderer, camera, context, swapChain);
            Console.WriteLine($"Resolution changed to: {currentResolution.Width}x{currentResolution.Height}");
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            this.Activate();  // Try explicitly activating the form
            this.Focus();
            Console.WriteLine("Form1 has focus: " + this.Focused);  // Debugging line

            Text = "Rotating Cube with Shader (SharpDX)";
            ClientSize = new System.Drawing.Size(800, 600);
            InitializeGraphics();
            Application.Idle += RenderLoop;
        }

        private ShaderResourceView pixelTextureView;
        private SwapChainDescription swapChainDesc;
        private SharpDX.Direct3D9.Effect shaderEffect;
        private void LoadShaderEffect()
        {
            // Locate the shader file

            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));
            var shaderPath = Path.Combine(projectRoot, "Shaders", "CubeShader.fx");

            // Debugging: Print the shader path
            Console.WriteLine("Looking for shader file at: " + shaderPath);

            if (!System.IO.File.Exists(shaderPath))
                throw new Exception("Shader file not found: " + shaderPath);


            // Load HLSL effect file
            shaderEffect = SharpDX.Direct3D9.Effect.FromFile(device9, shaderPath, ShaderFlags.None);
        }
        private Cube cube;
        private CubeDX9 cubeDX9;
        public void InitializeGraphics()
        {
            if(directx11)
            {

                swapChainDesc = new SwapChainDescription
                {
                    BufferCount = 1,
                    ModeDescription = new ModeDescription(ClientSize.Width, ClientSize.Height, new Rational(60, 1), SharpDX.DXGI.Format.R8G8B8A8_UNorm),
                    IsWindowed = true,
                    OutputHandle = Handle,
                    SampleDescription = new SampleDescription(1, 0),
                    SwapEffect = SharpDX.DXGI.SwapEffect.Discard,
                    Usage = SharpDX.DXGI.Usage.RenderTargetOutput
                }; 
                SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out device, out swapChain);
                context = device.ImmediateContext;

                var viewport = new Viewport(0, 0, ClientSize.Width, ClientSize.Height);
                renderer = new Renderer(device, swapChain, context);
                renderer.InitializeRenderTarget(viewport);

                // Navigate to the project root from the bin directory
                var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));

                // Combine the project root with the Shaders folder
                var shadersDirectory = Path.Combine(projectRoot, "Shaders");

                // Get the full path to the shader file
                var cubeShaderPath = Path.Combine(shadersDirectory, "CubeShader.hlsl");

                // Verify the file exists before proceeding
                if (!File.Exists(cubeShaderPath))
                {
                    throw new IOException($"Shader file not found at: {cubeShaderPath}");
                }

                shader11 = new Shader(device, cubeShaderPath); // Initialize shader with file path

                var solidCubeShaderPath = Path.Combine(shadersDirectory, "SolidCubeShader.hlsl");
                solidCubeShader = new Shader(device, solidCubeShaderPath); // Initialize shader with file pat


            }
            if (directx9)
            {
                // Correct PresentParameters for DirectX 9
                var presentParams = new SharpDX.Direct3D9.PresentParameters
                {
                    Windowed = true,
                    SwapEffect = SharpDX.Direct3D9.SwapEffect.Discard,
                    BackBufferFormat = SharpDX.Direct3D9.Format.X8R8G8B8, // Backbuffer format
                    BackBufferWidth = ClientSize.Width,
                    BackBufferHeight = ClientSize.Height,
                    EnableAutoDepthStencil = true,
                    AutoDepthStencilFormat = SharpDX.Direct3D9.Format.D16 // Depth-stencil format
                };

                // Create Direct3D 9 device
                var d3d = new SharpDX.Direct3D9.Direct3D();
                device9 = new SharpDX.Direct3D9.Device(
                    d3d,
                    0,
                    SharpDX.Direct3D9.DeviceType.Hardware,
                    Handle,
                    SharpDX.Direct3D9.CreateFlags.HardwareVertexProcessing,
                    presentParams
                );

                LoadShaderEffect();
                cubeDX9 = new CubeDX9(device9);
                cube = new Cube(device9);

                // Setting up viewport using the Device's existing Viewport property
                // Define the viewport
                var viewport = new Viewport
                {
                    X = 0,
                    Y = 0,
                    Width = ClientSize.Width,
                    Height = ClientSize.Height,
                };

                // Set the viewport to the device
                device9.Viewport = viewport;

                rendererDX9 = new RendererDX9(device9);
                // Initialize the renderer with width and height
                rendererDX9.InitializeRenderTarget(viewport.Width, viewport.Height);

                // Debugging: Output the parameters
                Console.WriteLine("Configured PresentParameters and initialized DirectX 9 rendering.");
                /*
                // Clear the screen
                device9.Clear(
                    SharpDX.Direct3D9.ClearFlags.Target | SharpDX.Direct3D9.ClearFlags.ZBuffer,
                    new SharpDX.ColorBGRA(0, 0, 0, 255), // Clear color
                    1.0f, // Depth buffer value
                    0 // Stencil buffer value
                );*/

                // Begin rendering scene
                //device9.BeginScene();

                Console.WriteLine("TRYING TO MAKE CUBE...");

                //device9.EndScene();
            }

            lineRenderer = new LineRenderer(device, context);


            cameraUI = new CameraUI(camera);

            camera = new Camera(new Vector3(0, 0, -5), Vector3.Zero, Vector3.UnitY, ClientSize.Width / (float)ClientSize.Height);


            //gameLogic.LoadEnemyCubes(cubes, collisionManager, cubePos, device);
            /*
            // Initialize the array and set its first element
            cubePos = new Vector3[3]; // Specify the size of the array
            cubePos[0] = new Vector3(-5.0f, 0.0f, 15f);
            cubePos[1] = new Vector3(0f, 0.0f, 15f);
            cubePos[2] = new Vector3(5.0f, 0.0f, 15f);
            // Initialize the cubes array with 2 elements
            cubes = new CubeWithCollider[3];
            cubes[0] = new CubeWithCollider(device, cubePos[0], 0.1f, collisionManager);
            cubes[1] = new CubeWithCollider(device, cubePos[1], 0.1f, collisionManager);
            cubes[2] = new CubeWithCollider(device, cubePos[2], 0.1f, collisionManager);
            */

            cubePos = new Vector3[3];//set the enemy cube count
            enemyCubes.InitializeEnemyCubes(cubePos, device, collisionManager);

            enemyCubes.SetEnemyCubeSizes();


            /*
            light = new Light(new Vector3(1, 1, 0), new Vector3(1, 0.5f, 1), 100f); // Light position, color, intensity

            LightData lightData = new LightData(light.Position, light.Color, light.Intensity);

            using (var stream = new DataStream(Utilities.SizeOf<LightData>(), true, true))
            {
                stream.Write(lightData);
                stream.Position = 0;

                lightBuffer = new SharpDX.Direct3D11.Buffer(device, stream, new BufferDescription
                {
                    SizeInBytes = Utilities.SizeOf<LightData>(),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.ConstantBuffer,
                    CpuAccessFlags = CpuAccessFlags.None
                });
            }*/

            // Initialize shadow system
            //shadow = new Shadow(device, ClientSize.Width, ClientSize.Height);
        }

        private Input input;
        private Random random;
        private bool hasShuffled = false;
        private SpriteTextRenderer spriteTextRenderer;
        private RenderTargetView renderTargetView; 
        private float cubeMovementZ = 0.0f;
        private const float movementSpeed = 0.01f; // Adjust the speed of movement
        private bool debug1 = false;
        private bool debug2 = false;
        private bool debug3 = false;
        private void RenderCubesDX9(RendererDX9 rendererDX9)
        {
            // Set transformation matrices
            var worldMatrix = Matrix.Identity; // Replace with actual world transformation
            var viewMatrix = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            var projectionMatrix = Matrix.PerspectiveFovLH((float)Math.PI / 4, ClientSize.Width / (float)ClientSize.Height, 1.0f, 100.0f);

            device9.SetTransform(TransformState.World, worldMatrix);
            device9.SetTransform(TransformState.View, viewMatrix);
            device9.SetTransform(TransformState.Projection, projectionMatrix);

            // Render cubes
            cube?.RenderDX9();
        }
        private float deltaTime;
        private void RenderLoop(object sender, EventArgs e)
        {


            var previousTime = DateTime.Now;

            while (Application.MessageLoop)
            {
                // Handle key press for c
                if (input.IsKeyPressed(Keys.C))
                {
                    if (!hasShuffled)
                    {
                        ShuffleCubeColors();
                        hasShuffled = true;
                    }
                }
                if (input.IsKeyNotPressed(Keys.C))
                {
                    hasShuffled = false;
                }

                camera.Update();
                cameraUI.UpdateUI(camera);

                // Clear screen
                if (directx11)
                {
                    // Clear screen with Direct3D11 (uses RawColor4)
                    renderer.ClearScreen(new RawColor4(0.0f, 0.5f, 0.5f, 1.0f));
                }

                if (directx9)
                {
                    // Clear screen with Direct3D9 (uses RawColorBGRA)
                    rendererDX9.ClearScreen(new RawColorBGRA(0, 128, 128, 255)); // Equivalent color in BGRA
                }
                UIManager.CalculateAndUpdateFPS();


                // Cube rotation and rendering
                rotationSpeed += 0.001f;
                // First Cube: Original Position (matrix1)
                var rotationX = Matrix.RotationX(rotationSpeed * 0.5f);
                var rotationY = Matrix.RotationY(rotationSpeed);
                var rotationZ = Matrix.RotationZ(rotationSpeed * 0.3f);

                // Combine rotations for local axis rotation
                localRot = rotationX * rotationY * rotationZ;

                var currentTime = DateTime.Now;
                deltaTime = (float)(currentTime - previousTime).TotalSeconds;
                previousTime = currentTime;
                // Render the projectiles
                // Update and render spawned cubes
                if (directx11)
                {
                    ///ENEMY RENDER


                    ///ENEMY RENDER END
                    ///



                    //TESTING*********
                    // Example usage of LineRenderer.RenderLine
                    var start = new Vector3(0, 0, 0); // Starting point of the line
                    var end = new Vector3(1, 1, 10);   // Ending point of the line
                    var viewProjectionMatrix = camera.GetViewProjection(); // Replace with your actual view-projection matrix

                    var greenColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); // Green color
                    var redColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); // Red color

                    //render a static line
                    lineRenderer.RenderLine(start, end, viewProjectionMatrix, greenColor, redColor);
                    lineRenderer.RenderWireframeCube(start, 2, viewProjectionMatrix, redColor, localRot);

                    // Render existing cubes (enemies)
                    shader11.SetShaders(context); // Set the cube shader
                    //render enemy cubes
                    enemyCubes.RenderEnemyCubes(localRot, worldMatrix, cubePos, context, camera, lineRenderer);

                    //give projectiles solid color
                    solidCubeShader.SetShaders(context);
                    //update projectiles
                    projectile.UpdateAndRenderProjectiles(deltaTime, camera, context, lineRenderer);

                     // Check for collisions
                    collisionManager.CheckCollisions();

                    gameInput.ShootGun(input, camera, device, projectile, collisionManager);
                    // Present rendered frame
                    renderer.Present();

                    Application.DoEvents();
                }
                if (directx9)
                {
                    //RenderCubes(localRot);
                    //LoadShaderEffect();
                    //RenderCubesDX9(rendererDX9);

                    RenderSingleCubeDX9();
                    device9.Present();
                    Application.DoEvents();
                }


            }
        }
        private Matrix worldMatrix2;
        private Matrix viewMatrix2;
        private Matrix projectionMatrix2;
        private void RenderSingleCubeDX9()
        {
            try
            {
                // Clear the screen and depth buffer
                device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new RawColorBGRA(0, 128, 128, 255), 1.0f, 0);
                device9.BeginScene();

                // Set shader and matrices
                rotationSpeed += 0.01f;

                // Apply rotation and move the cube forward 5 units
                worldMatrix2 = Matrix.RotationY(rotationSpeed) * Matrix.Translation(0, 0, 5);

                // Calculate the WorldViewProjection matrix
                Matrix worldViewProjection = worldMatrix2 * viewMatrix2 * projectionMatrix2;

                shaderEffect.SetValue("WorldViewProjection", worldViewProjection);

                Console.WriteLine("Setting technique...");
                shaderEffect.Technique = "RenderTechnique";
                Console.WriteLine("Technique set successfully.");

                int numPasses = shaderEffect.Begin(FX.DoNotSaveState);
                for (int pass = 0; pass < numPasses; pass++)
                {
                    shaderEffect.BeginPass(pass);
                    cubeDX9.Render();
                    //cube.RenderDX9();
                    shaderEffect.EndPass();
                }
                shaderEffect.End();

                Console.WriteLine("TRYING TO RENDER CUBE...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error rendering cube: {ex.Message}");
            }
            finally
            {
                device9.EndScene();
                device9.Present();
            }

        }



        private void ShuffleCubeColors()
        {
            foreach (var cube in enemyCubes.cubes)
            {
                cube.SetFaceColors(new[]
                {
                    RandomColor(), RandomColor(), RandomColor(),
                    RandomColor(), RandomColor(), RandomColor()
                });
            }
        }

        private Vector3 RandomColor()
        {
            return new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            foreach (var cube in enemyCubes.cubes)
            {
                cube.Dispose();
            }
            lightBuffer?.Dispose();
            swapChain?.Dispose();
            device?.Dispose();
            base.OnFormClosed(e);
        }
        private int resolutionIndex = 0;

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            this.Focus();
            this.Activate();
            input.KeyDown(e);

            //Console.WriteLine("KeyDown: Form has focus: " + this.Focused); // Debugging line
            gameInput.CameraMovementInput(input, camera);

            // Resolution handling (unchanged)
            if (input.IsKeyPressed(Keys.F1))
            {
                resolutionIndex = 0;//set res index
                ChangeRes();
            }
            else if (input.IsKeyPressed(Keys.F2))
            {
                resolutionIndex = 1;
                ChangeRes();
            }
            else if (input.IsKeyPressed(Keys.F3))
            {
                resolutionIndex = 2;
                ChangeRes();
            }
            else if (input.IsKeyPressed(Keys.F4))
            {
                resolutionIndex = 3; 
                ChangeRes();
            }

            if (e.KeyCode == Keys.M)
            {
                ShowUIForm();
            }

        }
        private void ChangeRes()
        {

            if (directx11)
                Resolution.ChangeResolution(this, resolutionIndex, availableResolutions, renderer, camera, context, swapChain);
            if (directx9)
                ResolutionDX9.ChangeResolution(this, resolutionIndex, availableResolutionsDX9, rendererDX9, camera, device9);
        }


        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            this.Focus();
            this.Activate();
            input.KeyUp(e);
            Console.WriteLine("KeyUp: Form has focus: " + this.Focused);  // Debugging line
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Focus();
            this.Activate();
            Console.WriteLine("Form has focus: " + this.Focused);  // Debugging line
        }

        private void Form1_Click(object sender, EventArgs e)
        {

            this.Focus();
            this.Activate();
            Console.WriteLine("Form has focus: " + this.Focused);  // Debugging line

            ShuffleCubeColors();
        }
    }
}
