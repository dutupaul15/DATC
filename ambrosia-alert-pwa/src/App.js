import React from "react";
import { BrowserRouter as Router, Route, Routes } from "react-router-dom";

import "./App.css";
import RegisterPage from "./components/register/Register";
import LoginPage from "./components/login/Login";

function App() {
  return (
    <div>
      <Router>
        <Routes>
          <Route element={<RegisterPage />} path="/" />
          <Route element={<LoginPage />} path="/login" />
        </Routes>
      </Router>
    </div>
  );
}

export default App;
