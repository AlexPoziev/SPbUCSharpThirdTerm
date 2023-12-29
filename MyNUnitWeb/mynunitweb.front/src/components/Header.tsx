import React, {useState, useEffect} from 'react';
import {AppBar, Grid, Typography, Toolbar} from '@mui/material';
import {Link} from "react-router-dom";

export const Header: React.FC = () => {
    return <div>
        <AppBar style={{position: "static", width: "100vw", maxWidth: "100%", backgroundColor: "gray"}}>
            <div className={"container"} style={{display: "flex", alignItems: "center"}}>
                <Toolbar>
                    <Grid container spacing={2} alignItems={"center"}>
                        <Grid item style={{marginRight: 1}}>
                            <Typography variant="h6" style={{color: 'white', fontFamily: "Helvetica"}}>
                                MyNUnitWeb
                            </Typography>
                        </Grid>
                        <Grid item>
                            <Link to={"addTests"}>
                                <Typography style={{color: 'white', fontFamily: "Helvetica"}}>
                                    Тестирование
                                </Typography>
                            </Link>
                        </Grid>
                        <Grid item>
                            <Link to={"history"}>
                                <Typography style={{color: 'white', fontFamily: "Helvetica"}}>
                                    История
                                </Typography>
                            </Link>
                        </Grid>
                    </Grid>
                </Toolbar>
            </div>
        </AppBar>
    </div>
};