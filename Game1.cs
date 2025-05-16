using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace idle;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private double gold;
    private double playerAtk;
    private double playerDef;
    private double elapsedTime;
    private double actionInterval;
    private bool isEnemyAlive;
    private double killCount;
    private double enemyMaxHp;
    private double enemyHp;
    private double enemyAtk;
    private double enemyDef;
    private double enemyScalar;
    private List<Button> buttons;
    private SpriteFont font;
    private MouseState mouseState;
    private MouseState previousMouseState;
    private Texture2D buildingTexture;
    private Texture2D wolfImage;

    private int buildingHeight = 50;
    private int buildingWidth = 800;
    private int buildingPadding = 10;
    private int buildingSpacing = 5;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        buttons = new List<Button>();
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        //Make game borderless full screen
        int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = screenWidth;
        _graphics.PreferredBackBufferHeight = screenHeight;
        Window.IsBorderless = true;
        Window.Position = new Point(0, 0);

        //initializing game variables
        gold = 0;
        playerAtk = 1;
        playerDef = 1;
        actionInterval = 2;
        enemyScalar = 1;
        killCount = 0;
        isEnemyAlive = false;

        //Add some textures
        buildingTexture = new Texture2D(GraphicsDevice, 1, 1);
        buildingTexture.SetData([Color.DimGray]);

        _graphics.ApplyChanges();

        List<String> lines= new List<String>();
        //lines.Add("Upgrade Click Power"); lines.Add("Line 2");
        buttons.Add(new Button("Sharpen Sword", 50, 1, 0, new Rectangle(
                                                    screenWidth-buildingWidth-buildingPadding,  //padding inside the building area
                                                    0*(buildingHeight + buildingSpacing)+buildingPadding,//spacing between buildings  
                                                    buildingWidth,   //subtract padding
                                                    buildingHeight  //height of each building
                                                    ),lines, buildingTexture));
        
        lines= new List<String>();
        buttons.Add(new Button("Raise Shield", 50, 0, 1, new Rectangle(
                                                    screenWidth-buildingWidth-buildingPadding,
                                                    1* (buildingHeight + buildingSpacing)+buildingPadding,
                                                    buildingWidth,  
                                                    buildingHeight
                                                    ),lines, buildingTexture));

        lines= new List<String>();
        buttons.Add(new Button("Train Harder", 200, 2, 2, new Rectangle(
                                                    screenWidth-buildingWidth-buildingPadding, 
                                                    2*(buildingHeight + buildingSpacing)+ buildingPadding,
                                                    buildingWidth,
                                                    buildingHeight
                                                    ),lines, buildingTexture));

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        font = Content.Load<SpriteFont>("MyCoolFont");

        wolfImage = Content.Load<Texture2D>("wolf");
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        mouseState = Mouse.GetState();
        Point mousePos = mouseState.Position;
        bool onClick = mouseState.LeftButton == ButtonState.Pressed && previousMouseState.LeftButton == ButtonState.Released;
        //logic for button clicks
        if (onClick)
        {
            foreach(Button button in buttons)
            {
                if(button.rectangle.Contains(mousePos))
                {
                    //purchases building, deducts cost, adds production
                    if (gold >= button.cost)
                    {
                        gold -= button.cost;
                        button.Purchase();
                        playerAtk += button.atk;
                        playerDef += button.def;          
                    }       
                }
            }  
        }
        previousMouseState = mouseState;

        //Executing gameplay loop every 2 seconds
        elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

        if (elapsedTime >= actionInterval)
        {
            if (isEnemyAlive)
            {
                attackSequence();
            }
            else
            {
                spawnEnemy();
            }
            elapsedTime = 0;
        }

        base.Update(gameTime);
    }

    private void spawnEnemy()
    {
        enemyHp = Math.Floor(2 + (enemyScalar * 3));
        enemyMaxHp = enemyHp;
        enemyAtk = Math.Floor(1 * enemyScalar);
        enemyDef = Math.Floor(0.65 * enemyScalar);

        enemyScalar += Math.Pow(1.05, killCount + 2) - 1;
        isEnemyAlive = true;
    }

    private void attackSequence()
    {
        if (enemyHp <= 0)
        {
            var rand = new Random();
            gold += 50 * enemyScalar * (1 + rand.NextDouble());
            isEnemyAlive = false;
            killCount += 1;
            return;
        }

        enemyHp = enemyHp - (playerAtk - enemyDef);
    }

    protected override void Draw(GameTime gameTime)
    {
        SpriteEffects flip = isEnemyAlive ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Vector2 squish = isEnemyAlive ? new Vector2(0.5f, 0.5f) : new Vector2(0.75f, 0.2f);

        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        string formattedMoney = gold > 999
            ? gold.ToString("E2")
            : gold.ToString("0");
        //add text to top left corner, peak UI right here
        _spriteBatch.DrawString(font, $"Gold: {formattedMoney}", new Vector2(30, 30), Color.Black);
        _spriteBatch.DrawString(font, $"Attack: {playerAtk}", new Vector2(30, 50), Color.Black);
        _spriteBatch.DrawString(font, $"Defense: {playerDef}", new Vector2(30, 70), Color.Black);
        _spriteBatch.DrawString(font, $"Wolves slain: {killCount}", new Vector2(30, 100), Color.Black);

        _spriteBatch.DrawString(font, $"Wolf Level: {Math.Floor(enemyScalar)}", new Vector2(500, 30), Color.Black);
        _spriteBatch.DrawString(font, $"Wolf HP: {enemyHp}/{enemyMaxHp}", new Vector2(500, 50), Color.Black);
        _spriteBatch.DrawString(font, $"Wolf Defense: {enemyDef}", new Vector2(500, 70), Color.Black);

        _spriteBatch.Draw(
            wolfImage,
            new Vector2(550, 300),
            null, // i have no idea why i need this here
            Color.White,
            0f,  // rotation
            new Vector2(
                wolfImage.Width,
                wolfImage.Height) * 0.5f, //origin
            squish, //scale
            flip,
            0.0f //depth
            );


        int i = 0;
        foreach (Button button in buttons)
        {
            int yPosition = i * (50 + 20) + 100;

            button.Draw(_spriteBatch, font);
            i++;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

