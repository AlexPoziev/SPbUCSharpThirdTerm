import React from 'react';
import logo from './logo.svg';
import './App.css';
import {Route, Routes} from "react-router-dom";
import {TestsHistory} from './components/TestsHistory';
import {Header} from './components/Header';

function App() {
  return (
      <div className="App">
          <Header />
          <Routes>
              <Route path="/" element={<TestsHistory />}/>
              <Route path="history" element={<TestsHistory />}/>
          </Routes>
      </div>
  );
}

export default App;
