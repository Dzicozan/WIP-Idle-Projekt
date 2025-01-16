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
    private double money;
    private double production;
    private int ticks;
    private List<Button> buttons;
    private SpriteFont font;
    private MouseState mouseState;
    private MouseState previousMouseState;
    private Texture2D buildingTexture;

    private int buildingHeight = 50;
    private int buildingWidth = 500;
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
        money = 510;
        production = 0;
        ticks = 60;

        //Add some textures
        buildingTexture = new Texture2D(GraphicsDevice, 1, 1);
        buildingTexture.SetData([Color.DimGray]);

        _graphics.ApplyChanges();

        List<String> lines= new List<String>();
        //lines.Add("Upgrade Click Power"); lines.Add("Line 2");
        buttons.Add(new Button("Bad Money Printer", 10, 1, new Rectangle(
                                                    screenWidth-buildingWidth-buildingPadding,  //padding inside the building area
                                                    0*(buildingHeight + buildingSpacing)+buildingPadding,//spacing between buildings  
                                                    buildingWidth,   //subtract padding
                                                    buildingHeight  //height of each building
                                                    ),lines, buildingTexture));
        
        lines= new List<String>();
        buttons.Add(new Button("Decent Money Printer", 50, 6, new Rectangle(
                                                    screenWidth-buildingWidth-buildingPadding,
                                                    1* (buildingHeight + buildingSpacing)+buildingPadding,
                                                    buildingWidth,  
                                                    buildingHeight
                                                    ),lines, buildingTexture));

        lines= new List<String>();
        buttons.Add(new Button("Awesome Money Printer", 200, 25, new Rectangle(
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
                    if(money>=button.cost)
                    {
                        money-=button.cost;
                        button.Purchase();
                        production+=button.rate;          
                    }       
                }
            }  
        }
        previousMouseState = mouseState;
        //dividing money per second by ticks per second to get money per tick, adding that value each tick
        //probably not clean because I'm hard defining that there's 60 ticks, but gametime assumes it updates 60 times a second so I'll get back to this later
        money += production / ticks;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        string formattedMoney = money > 999 
            ? money.ToString("E2") 
            : money.ToString("0");
        //add text to top left corner, peak UI right here
        _spriteBatch.DrawString(font, $"Dollars: {formattedMoney}", new Vector2(30, 30), Color.Black);
        _spriteBatch.DrawString(font, $"Dollars per second: {production}", new Vector2(30, 50), Color.Black);

        int i=0;
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
