import React from 'react';
import logo from './logo.svg';
import './App.css';
import {Route, Routes} from "react-router-dom";
import {TestsHistory} from './components/TestsHistory';
import {AddTests} from './components/AddTests';
import {Header} from './components/Header';

function App() {
  return (
      <div className="App" >
          <Header />
          <Routes>
              <Route path="history" element={<TestsHistory/>}/>
              <Route path="addTests" element={<AddTests/>}/>
          </Routes>
      </div>
  );
}

export default App;
