import { useEffect, useState } from 'react';
import './AdminPanel.css';
import { useNavigate } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';
import { AddButtonListReports, AddButtonUserProfile, AddButtonListEmployees, AddButtonAnalytics, AddButtonAnalyticsAllUserSurvey } from './funcs/Buttons';

function ListSurveys() {

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
    }, []);

    return (
        <>
            <div className="chart-container  animate-fade-in">
                <div className="profile-buttons">
                    {AddButtonListEmployees(navigate)}
                    {AddButtonUserProfile(navigate)}
                </div>
            </div>
            <div className="container">
                <div className="div-column profile-buttons">
                    {AddButtonAnalytics(navigate)}
                    {AddButtonAnalyticsAllUserSurvey(navigate)}
                    {AddButtonListReports(navigate)}
                </div>
            </div>
        </>
    )
}

export default ListSurveys;