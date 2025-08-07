using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PickMeAppV2.Modules
{
    public class Book
    {
        public String UserId { get; set; }
        public String PassengerName { get; set; }
        public String BookedDriverName { get; set; }
        public String BookedDriverId { get; set; }
        public int Location { get; set; }
        public String LocationName { get; set; }
        public int BookedSeats { get; set; }
        public int BookingId { get; set; }
        public string Status { get; set; }
        public DateTime BookingDT { get; set; }

        public float DestinationLat {  get; set; }
        public float DestinationLng {  get; set; }

        public float PickUpLat { get; set; }
        public float PickUpLng { get; set; }

        public float PickLat { get; set; }
        public float PickLng { get; set; }

        public string MapHtml
        {
            get
            {
                return $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <link rel='stylesheet' href='https://unpkg.com/leaflet@1.9.3/dist/leaflet.css' />
                            <script src='https://unpkg.com/leaflet@1.9.3/dist/leaflet.js'></script>
                            <style>
                                html, body, #map {{
                                    height: 100%;
                                    margin: 0;
                                    padding: 0;
                                }}
                            </style>
                        </head>
                        <body>
                            <div id='map'></div>
                            <script>
                                var start = [{PickLat}, {PickLng}];
                                var current = [{DestinationLat}, {DestinationLng}];

                                var map = L.map('map').setView(start, 15);

                                L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                                    attribution: '© OpenStreetMap contributors'
                                }}).addTo(map);

                                var polyline = L.polyline([start, current], {{ color: 'blue' }}).addTo(map);
                            </script>
                        </body>
                        </html>";
            }
        }


    }

}
