import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";

import "./App.css";
import RegisterPage from "./components/register/Register";
import LoginPage from "./components/login/Login";
import Dashboard from "./components/dashboard/Dashboard"; // import your Dashboard component

function App() {
  return (
    <div>
      <Router>
        <Routes>
          <Route element={<RegisterPage />} path="/" />
          <Route element={<LoginPage />} path="/login" />
          <Route element={<Dashboard />} path="/dashboard" />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
