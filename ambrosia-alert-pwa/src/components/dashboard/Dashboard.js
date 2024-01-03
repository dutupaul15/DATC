import * as React from "react";
import Container from "@mui/material/Container";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";
import Grid from "@mui/material/Grid";
import { MapContainer, TileLayer, Marker, Popup, useMap } from "react-leaflet";
import L from 'leaflet';
import 'leaflet/dist/leaflet.css'; // Import the Leaflet CSS

const myIcon = L.icon({
  iconUrl: '/location.png', // Replace with the path to your image
  iconSize: [50, 50], // Size of the icon
  iconAnchor: [22, 94], // Point of the icon which will correspond to marker's location
  popupAnchor: [-3, -76] // Point from which the popup should open relative to the iconAnchor
});

const DashboardPage = () => {
  const [alerts, setAlerts] = React.useState([]);
  const [position, setPosition] = React.useState(null);

  React.useEffect(() => {
    // Fetch alerts from your API
    fetch("/api/alerts")
      .then((response) => response.json())
      .then((data) => setAlerts(data));
  }, []);

  React.useEffect(() => {
    // Fetch your current location from the browser
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setPosition({
          lat: position.coords.latitude,
          lng: position.coords.longitude,
        });
      },
      (error) => {
        console.log(error);
      }
    );
  }, []);

  return (
    <Container maxWidth="lg">
      <Box
        sx={{
          marginTop:4,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          backgroundColor: "#cbfae1",
          borderRadius: "15px",
          border: 1,
          borderColor: "#cbfae1",
          display: "flex", // Add this line
          justifyContent: "center", // Add this line
          alignItems: "center" // Add this line
        }}
      >
        <Typography component="h1" variant="h5">
          Ambrosia Alert Dashboard
        </Typography>
        <Box
          sx={{
            marginTop: 3,
          }}
        >
          <Grid container spacing={2}>
            {alerts.map((alert) => (
              <Grid item key={alert.id} xs={12}>
                <Box
                  sx={{
                    backgroundColor: "white",
                    borderRadius: "10px",
                    padding: "10px",
                  }}
                >
                  <Typography variant="h6">{alert.title}</Typography>
                  <Typography variant="body1">{alert.description}</Typography>
                </Box>
              </Grid>
            ))}
          </Grid>
        </Box>
        <Box
          sx={{
            marginTop: 3,
            height: "500px", // Set the height of the map container
            width: "100%" // Set the width of the map container
          }}
        >
          {position && (
            <MapContainer center={position} zoom={13} style={{ height: "100%", width: "100%" }}>
              <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
              />
              <Marker position={position} icon={myIcon}>
                <Popup>
                  You are here!
                </Popup>
              </Marker>
              {alerts.map((alert) => (
                <Marker key={alert.id} position={alert.location} />
              ))}
            </MapContainer>
          )}
        </Box>
      </Box>
    </Container>
  );
};

export default DashboardPage;
