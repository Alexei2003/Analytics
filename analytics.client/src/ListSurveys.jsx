import { useEffect, useState } from 'react';
import './ListSurveys.css';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserRole } from './funcs/CookiesManager';
import { AddButtonAdminPanel, AddButtonUserProfile, AddButtonListEmployees, AddButtonSurveyCreator, AddButtonHRPanel } from './funcs/Buttons';

function ListSurveys() {
    const { type } = useParams();
    const [listSurveys, setListSurveys] = useState(null);

    const navigate = useNavigate();
    useEffect(() => {
        if (!getUserRole()) {
            navigate('/');
        }
        getListSurveys();
    }, []);

    function AddLine(survey) {
        if (!survey) {
            return null;
        }

        return (
            <tr onClick={() => navigateSurvey(survey.id)} key={survey.id} className="elemListSurveys">
                <td>
                    <label>{survey.title || 'Не указано'}</label>
                </td>
                <td>
                    <label>{survey.creationData || 'Не указано'}</label>
                </td>
                <td>
                    <label>{survey.endData || 'Не указано'}</label>
                </td>
            </tr>
        );
    }

    return (
        <>
            <div className="container">
                <div className="profile-buttons">
                    {AddButtonHRPanel(navigate)}
                    {AddButtonAdminPanel(navigate)}
                    {type === "-1" && (
                        <>
                            {AddButtonListEmployees(navigate)}
                        </>
                    )}
                    {AddButtonSurveyCreator(navigate)}
                    {AddButtonUserProfile(navigate)}
                </div>
                <table className="tableListSurveys">
                    <thead>
                        <tr className="elemListSurveys">
                            <th>
                                <label>Название</label>
                            </th>
                            <th>
                                <label>Дата создания</label>
                            </th>
                            <th>
                                <label>Дата окончания</label>
                            </th>
                        </tr>
                    </thead>

                    <tbody>
                        {listSurveys ? (
                            <>
                                {listSurveys.map(survey => (
                                    AddLine(survey)
                                ))}
                            </>
                        ) : null}
                    </tbody>
                </table>
            </div>
        </>
    );

    async function getListSurveys() {
        try {
            const token = localStorage.getItem('token');

            const response = await fetch(`listsurveys?type=${type}`, {
                method: 'GET',
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (response.ok) {
                const data = await response.json();
                setListSurveys(data);
            }

            if (response.status === 401) {
                console.error('Ошибка при получении данных:', response.statusText);
                navigate('/');
            }
        } catch (error) {
            console.error('Ошибка при отправке:', error);
        }
    }

    async function navigateSurvey(surveyId) {
        if (type === "0") {
            navigate(`/analyticsusersurvey/all/${surveyId}`);
        }
        else {
            if (type === "-1") {
                navigate(`/survey/${surveyId}`);
            }
            else {
                navigate(`/analyticsusersurvey/user/${surveyId}/${type}`);
            }
        }
    }


}

export default ListSurveys;