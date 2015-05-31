Instrument Price Viewer
================

.NET WPF application demonstrating using the reactive design patterns for achieving the great user experience.

The application contains the following views:
* Instrument Price Grid
  * current stock price is updated in realtime and throttled to avoid extra load on the UI thread
  * average price is calculated as a moving average of the last 5 prices
  * both the current and average prices are highlighted with green when increasing, and red when decreasing 
* Historical Instrument Prices Grid 
  * can be opened with the double-click at the instrument on the Instrument Price Grid)
  * shows a snapshot of the last 10 prices for the selected stock
  * the logic can be easily changed to keep updating the recent prices in realtime, if needed 

The prices are read automatically from the file
```
\bin\Debug\Data\Sample Data.txt
```
In addition to this, the application asynchronously writes to this file the randomly generated prices. If this is not the expected behaviour, please comment out loading of the module `InstrumentPriceFileAppenderModule` in `Bootstrapper.cs`:

    moduleCatalog.AddModule(typeof(InstrumentPriceFileAppenderModule), InitializationMode.WhenAvailable);
 
### Startup
* Restore nuget packages using the command-line batch
```
\tools\restore_nuget_packages.cmd
```
* To start the application set `InstrumentViewer` as a start project.
