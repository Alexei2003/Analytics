import React from 'react';
import { HashRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Authorization from './Authorization';
import ListEmployees from './ListEmployees';
import Employee from './Employee';
import Analytics from './Analytics';
import ListSurveys from './ListSurveys';
import Survey from './Survey';
import Page404 from './Page404';

import SurveyCreator from './SurveyCreator';

import AdminPanel from './AdminPanel';
import HRPanel from './HRPanel';

import AnalyticsAllUserSurvey from './AnalyticsAllUserSurvey';
import AnalyticsUserSurvey from './AnalyticsUserSurvey';

import ListReports from './ListReports';
import Report from './Report';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/404" element={<Page404 />} />
                <Route path="/" element={<Authorization />} />
                <Route path="/listemployees" element={<ListEmployees />} />
                <Route path="/employee/:id" element={<Employee />} />
                <Route path="/analytics" element={<Analytics />} />
                <Route path="/listsurveys/:type" element={<ListSurveys />} />
                <Route path="/survey/:id" element={<Survey />} />

                <Route path="/surveycreator" element={<SurveyCreator />} />

                <Route path="/hrpanel" element={<HRPanel />} />
                <Route path="/adminpanel" element={<AdminPanel />} />

                <Route path="/analyticsusersurvey/all/:id" element={<AnalyticsAllUserSurvey />} />
                <Route path="/analyticsusersurvey/user/:surveyId/:employeeId" element={<AnalyticsUserSurvey />} />

                <Route path="/listreports/:employeeId" element={<ListReports />} />
                <Route path="/report/:reportId/:employeeId" element={<Report />} />

                <Route path="*" element={<Navigate to="/" replace />} />
            </Routes>
        </Router>
    );
}

export default App;